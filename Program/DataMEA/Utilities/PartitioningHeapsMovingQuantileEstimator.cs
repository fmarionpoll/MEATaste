using System;

namespace MEATaste.DataMEA.Utilities
{
    /// <summary>
    /// A moving selector based on a partitioning heaps.
    /// Memory: O(windowSize).
    /// Add complexity: O(log(windowSize)).
    /// GetValue complexity: O(1).
    /// 
    /// Fast implementation of the moving quantile based on the partitioning heaps (2020)
    /// https://aakinshin.net/posts/partitioning-heaps-quantile-estimator/
    /// 
    /// <remarks>
    /// Based on the following paper:
    /// Hardle, W., and William Steiger. "Algorithm AS 296: Optimal median smoothing." Journal of the Royal Statistical Society.
    /// Series C (Applied Statistics) 44, no. 2 (1995): 258-264.
    /// </remarks>
    /// </summary>
    public class PartitioningHeapsMovingQuantileEstimator
    {
        private readonly int windowSize, k;
        private readonly double[] elementsOfThePartitioningHeap;
        private readonly int[] heapToElementIndex;
        private readonly int[] elementToHeapIndex;
        private readonly int rootHeapIndex, lowerHeapMaxSize;
        private readonly MovingQuantileEstimatorInitStrategy initStrategy;
        private int upperHeapSize, lowerHeapSize, totalElementCount;

        public PartitioningHeapsMovingQuantileEstimator(int windowSize, int k, MovingQuantileEstimatorInitStrategy initStrategy = MovingQuantileEstimatorInitStrategy.QuantileApproximation)
        {
            this.windowSize = windowSize;
            this.k = k;
            elementsOfThePartitioningHeap = new double[windowSize];
            heapToElementIndex = new int[windowSize];
            elementToHeapIndex = new int[windowSize];

            lowerHeapMaxSize = k;
            this.initStrategy = initStrategy;
            rootHeapIndex = k;
        }

        private void Swap(int heapIndex1, int heapIndex2)
        {
            int elementIndex1 = heapToElementIndex[heapIndex1];
            int elementIndex2 = heapToElementIndex[heapIndex2];
            double value1 = elementsOfThePartitioningHeap[heapIndex1];
            double value2 = elementsOfThePartitioningHeap[heapIndex2];

            elementsOfThePartitioningHeap[heapIndex1] = value2;
            elementsOfThePartitioningHeap[heapIndex2] = value1;
            heapToElementIndex[heapIndex1] = elementIndex2;
            heapToElementIndex[heapIndex2] = elementIndex1;
            elementToHeapIndex[elementIndex1] = heapIndex2;
            elementToHeapIndex[elementIndex2] = heapIndex1;
        }

        private void Sift(int heapIndex)
        {
            int SwapWithChildren(int heapCurrentIndex, int heapChildIndex1, int heapChildIndex2, bool isUpperHeap)
            {
                bool hasChild1 = rootHeapIndex - lowerHeapSize <= heapChildIndex1 && heapChildIndex1 <= rootHeapIndex + upperHeapSize;
                bool hasChild2 = rootHeapIndex - lowerHeapSize <= heapChildIndex2 && heapChildIndex2 <= rootHeapIndex + upperHeapSize;

                if (!hasChild1 && !hasChild2)
                    return heapCurrentIndex;

                if (hasChild1 && !hasChild2)
                {
                    if (elementsOfThePartitioningHeap[heapIndex] < elementsOfThePartitioningHeap[heapChildIndex1] && !isUpperHeap || elementsOfThePartitioningHeap[heapIndex] > elementsOfThePartitioningHeap[heapChildIndex1] && isUpperHeap)
                    {
                        Swap(heapIndex, heapChildIndex1);
                        return heapChildIndex1;
                    }
                    return heapCurrentIndex;
                }

                if (hasChild1 && hasChild2)
                {
                    if ((elementsOfThePartitioningHeap[heapIndex] < elementsOfThePartitioningHeap[heapChildIndex1] || elementsOfThePartitioningHeap[heapIndex] < elementsOfThePartitioningHeap[heapChildIndex2]) && !isUpperHeap ||
                        (elementsOfThePartitioningHeap[heapIndex] > elementsOfThePartitioningHeap[heapChildIndex1] || elementsOfThePartitioningHeap[heapIndex] > elementsOfThePartitioningHeap[heapChildIndex2]) && isUpperHeap)
                    {
                        int heapChildIndex0 =
                            elementsOfThePartitioningHeap[heapChildIndex1] > elementsOfThePartitioningHeap[heapChildIndex2] && !isUpperHeap ||
                            elementsOfThePartitioningHeap[heapChildIndex1] < elementsOfThePartitioningHeap[heapChildIndex2] && isUpperHeap
                                ? heapChildIndex1
                                : heapChildIndex2;
                        Swap(heapIndex, heapChildIndex0);
                        return heapChildIndex0;
                    }
                    return heapCurrentIndex;
                }

                throw new InvalidOperationException();
            }

            while (true)
            {
                if (heapIndex != rootHeapIndex)
                {
                    bool isUpHeap = heapIndex > rootHeapIndex;
                    int heapParentIndex = rootHeapIndex + (heapIndex - rootHeapIndex) / 2;
                    if (elementsOfThePartitioningHeap[heapParentIndex] < elementsOfThePartitioningHeap[heapIndex] && !isUpHeap || elementsOfThePartitioningHeap[heapParentIndex] > elementsOfThePartitioningHeap[heapIndex] && isUpHeap)
                    {
                        Swap(heapIndex, heapParentIndex);
                        heapIndex = heapParentIndex;
                        continue;
                    }
                    else
                    {
                        int heapChildIndex1 = rootHeapIndex + (heapIndex - rootHeapIndex) * 2;
                        int heapChildIndex2 = rootHeapIndex + (heapIndex - rootHeapIndex) * 2 + Math.Sign(heapIndex - rootHeapIndex);
                        int newHeapIndex = SwapWithChildren(heapIndex, heapChildIndex1, heapChildIndex2, isUpHeap);
                        if (newHeapIndex != heapIndex)
                        {
                            heapIndex = newHeapIndex;
                            continue;
                        }
                    }
                }
                else // heapIndex == rootHeapIndex
                {
                    if (lowerHeapSize > 0)
                    {
                        int newHeapIndex = SwapWithChildren(heapIndex, heapIndex - 1, -1, false);
                        if (newHeapIndex != heapIndex)
                        {
                            heapIndex = newHeapIndex;
                            continue;
                        }
                    }

                    if (upperHeapSize > 0)
                    {
                        int newHeapIndex = SwapWithChildren(heapIndex, heapIndex + 1, -1, true);
                        if (newHeapIndex != heapIndex)
                        {
                            heapIndex = newHeapIndex;
                            continue;
                        }
                    }
                }

                break;
            }
        }
        
        public void Add(double value)
        {
            int elementIndex = totalElementCount % windowSize;

            int Insert(int heapIndex)
            {
                elementsOfThePartitioningHeap[heapIndex] = value;
                heapToElementIndex[heapIndex] = elementIndex;
                elementToHeapIndex[elementIndex] = heapIndex;
                return heapIndex;
            }

            if (totalElementCount++ < windowSize) // Heap is not full
            {
                if (totalElementCount == 1) // First element
                {
                    Insert(rootHeapIndex);
                }
                else
                {
                    bool quantileApproximationCondition =
                        initStrategy == MovingQuantileEstimatorInitStrategy.QuantileApproximation &&
                        lowerHeapSize < k * totalElementCount / windowSize ||
                        initStrategy == MovingQuantileEstimatorInitStrategy.OrderStatistics;
                    if (lowerHeapSize < lowerHeapMaxSize && quantileApproximationCondition)
                    {
                        lowerHeapSize++;
                        int heapIndex = Insert(rootHeapIndex - lowerHeapSize);
                        Sift(heapIndex);
                    }
                    else
                    {
                        upperHeapSize++;
                        int heapIndex = Insert(rootHeapIndex + upperHeapSize);
                        Sift(heapIndex);
                    }
                }
            }
            else
            {
                // Replace old element
                int heapIndex = elementToHeapIndex[elementIndex];
                Insert(heapIndex);
                Sift(heapIndex);
            }
        }
        
        public double GetQuantile()
        {
            if (totalElementCount == 0)
                throw new IndexOutOfRangeException("There are no any values");
            if (initStrategy == MovingQuantileEstimatorInitStrategy.OrderStatistics && k >= totalElementCount)
                throw new IndexOutOfRangeException($"Not enough values (n = {totalElementCount}, k = {k})");
            return elementsOfThePartitioningHeap[rootHeapIndex];
        }
    }
}


namespace TasteMEA.DataMEA.Utilies
{
    public static class Filter
    {
		/*
		Usui S.and Amidror I. (1982)
		Digital low-pass differentiation for biological signal processing.
		IEEE Trans.Biomed.Eng.  20 (10) 686-693

		"New algorithms: 2f3", cf Table I, p 691
		 y(k) =  (1/6T)*
				   [  (x(k + 1) + x(k + 2) + x(k + 3))    (a)
					- (x(k - 1) + x(k - 2) + x(k - 3) ) ] (b)

			The expression is evaluated in two steps. 
			Expression (a) equals expression (b) * (-1) 4 steps later. The
			algorithm use this to evaluate this	expression only once, 
			but stores it in y(k+4) and subtract it
			from y(k), using ax, bx, and dx to store x(k+1), x(k+2)
			and x(k+3).
		*/
		public static double[] BDerivFast(double[] dataIn, int rowLength)
        {
			double[] dataOut = new double[rowLength];
			int span = 4;

			double ax = dataIn[0];   // k-3
			double bx = dataIn[1];   // k-2
			double cx = dataIn[2];   // k-1
			double sumk = ax + bx + cx;
			dataOut[span] = (ushort) -sumk;

			for (int k = span; k < rowLength - span; k++)
            {
				ax = bx;			// k+1
				bx = cx;			// k+2
				cx = dataIn[k-1];     // k+3
				sumk = (ax + bx + cx)/ 6 ;
				dataOut[k] =  -sumk;   // y(k+4) = -SUM
				dataOut[k-span] += sumk;		
            }

			ZeroesStartAndEnd(dataOut, rowLength, span*2);
			return dataOut;
        }

		public static double[] BDeriv(double[] dataIn, int rowLength)
		{
			double[] dataOut = new double[rowLength];
			int span = 4;
			for (int k = span; k < rowLength - span; k++)
			{
				dataOut[k] = (dataIn[k + 1] + dataIn[k + 2] + dataIn[k + 3]
					- dataIn[k - 1] - dataIn[k - 2] - dataIn[k - 3])/6;
			}
			ZeroesStartAndEnd(dataOut, rowLength, span);

			return dataOut;
		}

		private static void ZeroesStartAndEnd(double[] dataOut, int rowLength, int span)
		{
			int j = rowLength - 1;
			for (int i = 0; i <= span; i++, j--)
			{
				dataOut[i] = 0;
				dataOut[j] = 0;
			}
		}

        /*
		public static double[] BMedian(double[] source, int rowLength, int nbspan)
		{
			double[] dataOut = new double[rowLength];
			int m_parray_size = nbspan * 2 + 1;
			double[] m_parraySorted = new double[m_parray_size];
			double[] m_parrayCircular = new double[m_parray_size];
			int i;
			for (i = 0; i < m_parray_size; i++)
			{
				m_parraySorted [i] = source[i]; 
				m_parrayCircular[i] = source[i];
			}

			// sort m_parraySorted into ascending order using heapsort algorithm
			// cf Numerical recipes Press et al.1986, pp 231
			// "l"= index which will be decremented from initial value down to 0 during
			// 'hiring' (heap creation) phase. Once it reaches 0, the index "ir" will be
			// decremented from its initial value down to 0 during the 'retirement-and-
			// promotion' (heap selection) phase.

			int l = nbspan + 1;              // temp index
			int ir = m_parray_size - 1;     // temp index
			double val;                      // temp storage

			for (; ; )                      // pseudo-loop over m_parraySorted
			{
				// -------------------------
				if (l > 0)                  // still in hiring phase
				{
					l--;
					val = m_parraySorted[l];
				}
				else                        // in retirement-and-promotion phase
				{
					val = m_parraySorted [ir];       // clear a space at the end of the array
					m_parraySorted [ir] = m_parraySorted[0]; // retire the top of the heap into it
					ir--;                   // decrease the size of the corporation
					if (ir == 0)            // done with the last promotion
					{
						m_parraySorted[0] = val;    // the least competent worker of all
						break;              // exit the sorting algorithm
					}
				}
				// -------------------------
				i = l + 1;                  // wether we are in the hiring or promotion, we
				int jj1 = i + i;            // here set up to sift down element to its
											// proper level
				while (jj1 - 1 <= ir)
				{
					if (jj1 - 1 < ir)
					{
						if (m_parraySorted[jj1 - 1] < m_parraySorted [jj1])
							jj1++;          // compare to the better underlining
					}
					if (val < m_parraySorted [jj1 - 1])// demote value
					{
						m_parraySorted [i - 1] = m_parraySorted [jj1 - 1];
						i = jj1;
						jj1 = jj1 + jj1;
					}                       // this is value's level. Set j to terminate the
					else                    // sift-down
						jj1 = ir + 2;
				}
				m_parraySorted [i - 1] = val;        // put value into its slot
			}
			// end of initial sort

			int lp = 0;                 // first data point
			int lp_next = lp + nbspan;	// last point
			int i_parray_circular = m_parray_size - 1; // point on the last item so that first operation is blank

			for (auto icx = cx; icx > 0; icx--, lp += lp_source_offset_nextpoint, lp_next += lp_source_offset_nextpoint, lp_dest++)
			{
				const auto oldvalue = *(m_parrayCircular + i_parray_circular);  // old value
				const auto newvalue = *lp_next;                                 // new value to insert into array
				*(m_parrayCircular + i_parray_circular) = newvalue; // save new value into circular array

				// update circular array pointer
				i_parray_circular++;
				if (i_parray_circular >= m_parray_size)
					i_parray_circular = 0;

				// locate position of old value to discard
				// use bisection - cf Numerical Recipes pp 90
				// on exit, j= index of oldvalue

				// binary search
				// Herbert Schildt: C the complete reference McGraw Hill, 1987, pp 488
				auto jhigh = m_parray_size - 1; // upper index
				auto jlow = 0;                  // mid point index
				auto jj2 = (jlow + jhigh) / 2;
				while (jlow <= jhigh)
				{
					jj2 = (jlow + jhigh) / 2;
					if (oldvalue > *(m_parraySorted + jj2))
						jlow = jj2 + 1;
					else if (oldvalue < *(m_parraySorted + jj2))
						jhigh = jj2 - 1;
					else
						break;
				}

				// insert new value in the correct position

				// case 1: search (and replace) towards higher values
				if (newvalue > *(m_parraySorted + jj2))
				{
					auto j = jj2;
					for (auto k = jj2; newvalue > *(m_parraySorted + k); k++, j++)
					{
						if (k == m_parray_size)
							break;
						*(m_parraySorted + j) = *(m_parraySorted + j + 1);
					}
					*(m_parraySorted + j - 1) = newvalue;
				}

				// case 2: search (and replace) towards lower values
				else if (newvalue < *(m_parraySorted + jj2))
				{
					auto j = jj2;
					for (auto k = jj2; newvalue < *(m_parraySorted + k); k--, j--)
					{
						if (j == 0)
						{
							if (newvalue < *m_parraySorted)
								j--;
							break;
						}
						*(m_parraySorted + j) = *(m_parraySorted + j - 1);
					}
					*(m_parraySorted + j + 1) = newvalue;
				}

				// case 3: already found!
				else
					*(m_parraySorted + jj2) = newvalue;

				// save median value in the output array
				*lp_dest = *lp - *(m_parraySorted + nbspan);
				ASSERT(lp >= min_lp_source);
				ASSERT(lp <= max_lp_source);
			}
			return dataOut;
		}
        */

	}

    /// <summary>
    /// A moving selector based on a partitioning heaps.
    /// Memory: O(windowSize).
    /// Add complexity: O(log(windowSize)).
    /// GetValue complexity: O(1).
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
        private readonly double[] h;
        private readonly int[] heapToElementIndex;
        private readonly int[] elementToHeapIndex;
        private readonly int rootHeapIndex, lowerHeapMaxSize;
        private readonly MovingQuantileEstimatorInitStrategy initStrategy;
        private int upperHeapSize, lowerHeapSize, totalElementCount;

        public PartitioningHeapsMovingQuantileEstimator(int windowSize, int k, MovingQuantileEstimatorInitStrategy initStrategy = MovingQuantileEstimatorInitStrategy.QuantileApproximation)
        {
            this.windowSize = windowSize;
            this.k = k;
            h = new double[windowSize];
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
            double value1 = h[heapIndex1];
            double value2 = h[heapIndex2];

            h[heapIndex1] = value2;
            h[heapIndex2] = value1;
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
                    if (h[heapIndex] < h[heapChildIndex1] && !isUpperHeap || h[heapIndex] > h[heapChildIndex1] && isUpperHeap)
                    {
                        Swap(heapIndex, heapChildIndex1);
                        return heapChildIndex1;
                    }
                    return heapCurrentIndex;
                }

                if (hasChild1 && hasChild2)
                {
                    if ((h[heapIndex] < h[heapChildIndex1] || h[heapIndex] < h[heapChildIndex2]) && !isUpperHeap ||
                        (h[heapIndex] > h[heapChildIndex1] || h[heapIndex] > h[heapChildIndex2]) && isUpperHeap)
                    {
                        int heapChildIndex0 =
                            h[heapChildIndex1] > h[heapChildIndex2] && !isUpperHeap ||
                            h[heapChildIndex1] < h[heapChildIndex2] && isUpperHeap
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
                    if (h[heapParentIndex] < h[heapIndex] && !isUpHeap || h[heapParentIndex] > h[heapIndex] && isUpHeap)
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
                h[heapIndex] = value;
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
            return h[rootHeapIndex];
        }
    }

    /// <summary>
    /// Defines how a moving quantile estimator calculates the target quantile value
    /// when the total number of elements is less than the window size
    /// </summary>
    public enum MovingQuantileEstimatorInitStrategy
    {
        /// <summary>
        /// Approximate the target quantile.
        ///
        /// <example>
        /// windowSize = 5, k = 2 (the median)
        /// If the total number of elements equals 3, the median (k = 1) will be returned 
        /// </example> 
        /// </summary>
        QuantileApproximation,

        /// <summary>
        /// Return the requested order statistics
        ///
        /// <example>
        /// windowSize = 5, k = 2
        /// If the total number of elements equals 3, the largest element (k = 2) will be returned 
        /// </example> 
        /// </summary>
        OrderStatistics
    }
	
}

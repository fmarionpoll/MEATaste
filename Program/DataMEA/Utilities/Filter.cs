namespace MEATaste.DataMEA.Utilities
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

		public static double[] BMedian(double[] dataIn, int rowLength, int nbspan)
		{
			double[] dataOut = new double[rowLength];
			int m_parray_size = nbspan * 2 + 1;
			double[] m_parraySorted = new double[m_parray_size];
			double[] m_parrayCircular = new double[m_parray_size];
			int i;
			for (i = 0; i < m_parray_size; i++)
			{
				m_parraySorted [i] = dataIn[i]; 
				m_parrayCircular[i] = dataIn[i];
			}

			// sort m_parraySorted into ascending order using heapsort algorithm
			// cf Numerical recipes Press et al. 1986, pp 231
			// "l"= index which will be decremented from initial value down to 0 during
			// 'hiring' (heap creation) phase. Once it reaches 0, the index "ir" will be
			// decremented from its initial value down to 0 during the 'retirement-and-
			// promotion' (heap selection) phase.

			int l = nbspan + 1;             // temp index
			int ir = m_parray_size - 1;     // temp index
			double val;                     // temp storage

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

			int i_parray_circular = m_parray_size - 1; // point on the last item so that first operation is blank

            for (int icx = rowLength-1; icx > 0; icx--)
            {
                double oldvalue = m_parrayCircular[i_parray_circular];  // old value
				double newvalue = dataIn[icx];                     // new value to insert into array
				m_parrayCircular [i_parray_circular] = newvalue; // save new value into circular array

				// update circular array pointer
				i_parray_circular++;
				if (i_parray_circular >= m_parray_size)
					i_parray_circular = 0;

				// locate position of old value to discard
				// use bisection - cf Numerical Recipes pp 90
				// on exit, j= index of oldvalue

				// binary search
				// Herbert Schildt: C the complete reference McGraw Hill, 1987, pp 488
				int jhigh = m_parray_size - 1; // upper index
				int jlow = 0;                  // mid point index
				int jj2 = (jlow + jhigh) / 2;
				while (jlow <= jhigh)
				{
					jj2 = (jlow + jhigh) / 2;
					if (oldvalue > m_parraySorted [jj2])
						jlow = jj2 + 1;
					else if (oldvalue < m_parraySorted [jj2])
						jhigh = jj2 - 1;
					else
						break;
				}

				// insert new value in the correct position

				// case 1: search (and replace) towards higher values
				if (newvalue > m_parraySorted [jj2])
				{
					int j = jj2;
					for (int k = jj2; newvalue > m_parraySorted [k]; k++, j++)
					{
						if (k == m_parray_size -1)
							break;
						m_parraySorted [j] = m_parraySorted [j + 1];
					}
					m_parraySorted [j - 1] = newvalue;
				}

				// case 2: search (and replace) towards lower values
				else if (newvalue < m_parraySorted[jj2])
				{
					int j = jj2;
					for (int k = jj2; newvalue < m_parraySorted [k]; k--, j--)
					{
						if (j == 0)
						{
							if (newvalue < m_parraySorted[0])
								j--;
							break;
						}
						m_parraySorted[j] = m_parraySorted [j - 1];
					}
					m_parraySorted[j + 1] = newvalue;
				}

				// case 3: already found!
				else
					m_parraySorted[jj2] = newvalue;

                // save median value in the output array
                dataOut[icx] = dataIn[icx] - m_parraySorted[nbspan-1];
			}
			return dataOut;
		}
        

	}
}

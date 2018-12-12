using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;


namespace SS_OpenCV
{
    public static class PuzzleClassic
    {
        private static uint ComputeClosure(IDictionary<uint, uint> equivalenceTable, uint key)
        {
            var path = new LinkedList<uint>();

            uint closure = key, currentKey = key;
            while (equivalenceTable.TryGetValue(currentKey, out var value))
            {
                // Already has an entry with the same key
                path.AddFirst(currentKey);
                closure = value;
                currentKey = value;
            }

            foreach (var k in path)
            {
                equivalenceTable[k] = closure;
            }

            return closure;
        }


        private static uint InsertEquivalence(IDictionary<uint, uint> equivalenceTable, uint key, uint value)
        {
            if (equivalenceTable.TryGetValue(key, out var old))
            {
                // Already has an entry with the same key
                if (value < old)
                {
                    var closure = ComputeClosure(equivalenceTable, value);
                    equivalenceTable[key] = closure;
                    InsertEquivalence(equivalenceTable, old, closure);
                    return closure;
                }
                else if (old < value)
                {
                    InsertEquivalence(equivalenceTable, value, old);
                    return old;
                }

                return old;
            }
            else
            {
                var closure = ComputeClosure(equivalenceTable, value);
                equivalenceTable.Add(key, closure);
                return closure;
            }
        }

        public static uint[,] GetLabelsClassic(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;

                uint[,] labels = new uint[img.Width, img.Height];

                byte[] background = new byte[3];
                background[0] = dataPtr[0];
                background[1] = dataPtr[1];
                background[2] = dataPtr[2];

                Dictionary<uint, uint> equivalenceTable = new Dictionary<uint, uint>();

                uint nextLabel = 1, currentLabel = 0;

                dataPtr += nChan + step;

                // First passage
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (dataPtr[0] != background[0] || dataPtr[1] != background[1] || dataPtr[2] != background[2])
                        {
                            uint minNeighLabel = uint.MaxValue;
                            // Verificar se existe label menor ao lado
                            byte* neigh = dataPtr - nChan - step;
                            bool not_bg = (neigh[0] != background[0] || neigh[1] != background[1] ||
                                           neigh[2] != background[2]);
                            if (not_bg)
                            {
                                minNeighLabel = labels[x - 1, y - 1];
                            }

                            neigh = dataPtr - step;
                            not_bg = (neigh[0] != background[0] || neigh[1] != background[1] ||
                                      neigh[2] != background[2]);
                            if (not_bg)
                            {
                                currentLabel = labels[x, y - 1];
                                if (minNeighLabel != uint.MaxValue && minNeighLabel != currentLabel)
                                {
                                    if (currentLabel < minNeighLabel)
                                    {
                                        minNeighLabel = InsertEquivalence(equivalenceTable, minNeighLabel,
                                            currentLabel);
                                    }
                                    else if (currentLabel > minNeighLabel)
                                    {
                                        InsertEquivalence(equivalenceTable, currentLabel, minNeighLabel);
                                    }
                                }
                                else
                                {
                                    minNeighLabel = currentLabel;
                                }
                            }

                            neigh = dataPtr - step + nChan;
                            not_bg = (neigh[0] != background[0] || neigh[1] != background[1] ||
                                      neigh[2] != background[2]);
                            if (not_bg)
                            {
                                currentLabel = labels[x + 1, y - 1];
                                if (minNeighLabel != uint.MaxValue && minNeighLabel != currentLabel)
                                {
                                    if (currentLabel < minNeighLabel)
                                    {
                                        minNeighLabel = InsertEquivalence(equivalenceTable, minNeighLabel,
                                            currentLabel);
                                    }
                                    else if (currentLabel > minNeighLabel)
                                    {
                                        InsertEquivalence(equivalenceTable, currentLabel, minNeighLabel);
                                    }
                                }
                                else
                                {
                                    minNeighLabel = currentLabel;
                                }
                            }

                            neigh = dataPtr - nChan;
                            not_bg = (neigh[0] != background[0] || neigh[1] != background[1] ||
                                      neigh[2] != background[2]);
                            if (not_bg)
                            {
                                currentLabel = labels[x - 1, y];
                                if (minNeighLabel != uint.MaxValue && minNeighLabel != currentLabel)
                                {
                                    if (currentLabel < minNeighLabel)
                                    {
                                        minNeighLabel = InsertEquivalence(equivalenceTable, minNeighLabel,
                                            currentLabel);
                                    }
                                    else if (currentLabel > minNeighLabel)
                                    {
                                        InsertEquivalence(equivalenceTable, currentLabel, minNeighLabel);
                                    }
                                }
                                else
                                {
                                    minNeighLabel = currentLabel;
                                }
                            }

                            if (minNeighLabel != uint.MaxValue)
                            {
                                labels[x, y] = minNeighLabel;
                            }
                            else
                            {
                                labels[x, y] = nextLabel++;
                            }

                        }

                        dataPtr += nChan;
                    }

                    dataPtr += 2 * nChan + padding;
                }


                if (equivalenceTable.Count <= 0) return labels;
                {
                    // Compute transitive closure
                    bool change;
                    
                    do
                    {
                        change = false;
                        var list = equivalenceTable.ToList();
                        foreach (KeyValuePair<uint, uint> entry in list)
                        {
                            uint closure = ComputeClosure(equivalenceTable, entry.Key);
                            change = (entry.Value != closure);
                        }
                    } while (change);

                    dataPtr = (byte*)m.imageData.ToPointer() + nChan + step;
                    uint currentClosure = 0;
                    currentLabel = 0;

                    // Second passage
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            if (labels[x, y] != currentLabel)
                            {
                                currentClosure = equivalenceTable.TryGetValue(labels[x, y], out var value) ? value : labels[x, y];

                                currentLabel = labels[x, y];
                            }

                            labels[x, y] = currentClosure;


                            dataPtr += nChan;
                        }

                        dataPtr += 2 * nChan + padding;
                    }
                }

                return labels;
            }
        }


    }

}

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

public static class PuzzleClassic
{


        public static uint computeClosure(Dictionary<uint, uint> equivalence_table, uint key)
        {
            LinkedList<uint> path = new LinkedList<uint>();

            uint closure = key, value, current_key = key;
            while (equivalence_table.TryGetValue(current_key, out value))
            { // Already has an entry with the same key
                path.AddFirst(current_key);
                closure = value;
                current_key = value;
            }

            foreach (uint k in path)
            {
                equivalence_table[k] = closure;
            }

            return closure;
        }


    public static uint insertEquivalence(Dictionary<uint, uint> equivalence_table, uint key, uint value)
        {
            uint old;
            if (equivalence_table.TryGetValue(key, out old))
            { // Already has an entry with the same key
                if (value < old)
                {
                    uint closure = computeClosure(equivalence_table, value);
                    equivalence_table[key] = closure;
                    insertEquivalence(equivalence_table, old, closure);
                    return closure;
                }
                else if (old < value)
                {
                    insertEquivalence(equivalence_table, value, old);
                    return old;
                }
                return old;
            }
            else
            {
                uint closure = computeClosure(equivalence_table, value);
                equivalence_table.Add(key, closure);
                return closure;
            }
        }

    public static uint[,] getLabelsClassic(Image<Bgr, byte> img)
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

                uint[,] labels = new uint[img.Width, img.Height]; // É inicializado a 0?

                byte[] background = new byte[3];
                background[0] = dataPtr[0];
                background[1] = dataPtr[1];
                background[2] = dataPtr[2];

                Dictionary<uint, uint> equivalence_table = new Dictionary<uint, uint>();

                uint next_label = 1, current_label = 0;

                dataPtr += nChan + step;

                // First passage
                for (int y = 1; y < height - 1; y++)
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        if (dataPtr[0] != background[0] || dataPtr[1] != background[1] || dataPtr[2] != background[2])
                        {
                            uint min_neigh_label = UInt32.MaxValue;
                            // Verificar se existe label menor ao lado
                            byte* neigh = dataPtr - nChan - step;
                            bool not_bg = (neigh[0] != background[0] || neigh[1] != background[1] || neigh[2] != background[2]);
                            if (not_bg)
                            {
                                min_neigh_label = labels[x - 1, y - 1];
                            }

                            neigh = dataPtr - step;
                            not_bg = (neigh[0] != background[0] || neigh[1] != background[1] || neigh[2] != background[2]);
                            if (not_bg)
                            {
                                current_label = labels[x, y - 1];
                                if (min_neigh_label != UInt32.MaxValue && min_neigh_label != current_label)
                                {
                                    if (current_label < min_neigh_label)
                                    {
                                        min_neigh_label = insertEquivalence(equivalence_table, min_neigh_label, current_label);
                                    }
                                    else if (current_label > min_neigh_label)
                                    {
                                        insertEquivalence(equivalence_table, current_label, min_neigh_label);
                                    }
                                }
                                else
                                {
                                    min_neigh_label = current_label;
                                }
                            }

                            neigh = dataPtr - step + nChan;
                            not_bg = (neigh[0] != background[0] || neigh[1] != background[1] || neigh[2] != background[2]);
                            if (not_bg)
                            {
                                current_label = labels[x + 1, y - 1];
                                if (min_neigh_label != UInt32.MaxValue && min_neigh_label != current_label)
                                {
                                    if (current_label < min_neigh_label)
                                    {
                                        min_neigh_label = insertEquivalence(equivalence_table, min_neigh_label, current_label);
                                    }
                                    else if (current_label > min_neigh_label)
                                    {
                                        insertEquivalence(equivalence_table, current_label, min_neigh_label);
                                    }
                                }
                                else
                                {
                                    min_neigh_label = current_label;
                                }
                            }

                            neigh = dataPtr - nChan;
                            not_bg = (neigh[0] != background[0] || neigh[1] != background[1] || neigh[2] != background[2]);
                            if (not_bg)
                            {
                                current_label = labels[x - 1, y];
                                if (min_neigh_label != UInt32.MaxValue && min_neigh_label != current_label)
                                {
                                    if (current_label < min_neigh_label)
                                    {
                                        min_neigh_label = insertEquivalence(equivalence_table, min_neigh_label, current_label);
                                    }
                                    else if (current_label > min_neigh_label)
                                    {
                                        insertEquivalence(equivalence_table, current_label, min_neigh_label);
                                    }
                                }
                                else
                                {
                                    min_neigh_label = current_label;
                                }
                            }

                            if (min_neigh_label != UInt32.MaxValue)
                            {
                                labels[x, y] = min_neigh_label;
                            }
                            else
                            {
                                labels[x, y] = next_label++;
                            }

                        }
                        dataPtr += nChan;
                    }
                    dataPtr += 2 * nChan + padding;
                }

                foreach(KeyValuePair<uint, uint> e in equivalence_table)
            {
                Console.WriteLine(e.Key + " -> " + e.Value);
            }
                

                if (equivalence_table.Count > 0)
                {

                    // Compute transitive closure
                    bool change = false;
                    do
                    {
                    change = false;
                    var list = equivalence_table.ToList();
                        foreach (KeyValuePair<uint, uint> entry in list)
                        {
                            uint closure = computeClosure(equivalence_table, entry.Key);
                            change = (entry.Value != closure);
                        }
                    } while (change);
                    dataPtr = (byte*)m.imageData.ToPointer() + nChan + step;
                    uint current_closure = 0;
                    current_label = 0;

                    // Second passage
                    for (int y = 1; y < height - 1; y++)
                    {
                        for (int x = 1; x < width - 1; x++)
                        {
                            if (labels[x, y] != current_label)
                            {
                                uint value;
                                if (equivalence_table.TryGetValue(labels[x, y], out value))
                                {
                                    current_closure = value;
                                }
                                else
                                {
                                    current_closure = labels[x, y];
                                }
                                current_label = labels[x, y];
                            }
                            
                                labels[x, y] = current_closure;
                            

                            dataPtr += nChan;
                        }
                        dataPtr += 2 * nChan + padding;
                    }
                }

                return labels;
            }
        }



    
}

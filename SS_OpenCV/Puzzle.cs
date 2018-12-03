using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SS_OpenCV
{
    
    public struct SideValues
    {
        public readonly int Side;
        public readonly double Points;
            
        public SideValues(int side, double diff)
        {
            Side = side;
            Points = diff;
        }
    }
    
    internal class Puzzle
    {
        private const int MaxNumberPieces = 10;
        private const double RadToDegreeFactor = 180.0 / Math.PI;
        
        private readonly Image<Bgr, byte> _originalImage;
        private readonly List<Image<Bgr, byte>> _imagesPieces;
        
        private readonly uint[,] _labels;
        private readonly uint[] _labelsIndex;
        private int _currLabelIndex;

        private const uint BASE_VALUE = 0;

        // TODO for testing
        private int pieceID;
        private int partsID;
        
        private int _backgroundB = -1, _backgroundG = -1, _backgroundR = -1;

        public Puzzle(Image<Bgr, byte> img)
        {
            _originalImage = img;
            _labels = new uint[img.Width, img.Height];
            _labelsIndex = new uint[MaxNumberPieces];
            _currLabelIndex = 0;
            _imagesPieces = new List<Image<Bgr, byte>>();

            unsafe
            {
                MIplImage m = _originalImage.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                _backgroundB = dataPtrOriginal[0];
                _backgroundG = dataPtrOriginal[1];
                _backgroundR = dataPtrOriginal[2];
            }

            pieceID = 0;
            partsID = 0;

            //GetLabels();
            _labels = PuzzleClassic.getLabelsClassic(img);
        }

        private static int RadsToDegrees(double rads)
        {
            return (int)Math.Round(rads * RadToDegreeFactor);
        }

        private void GetLabels()
        {
            unsafe
            {
                MIplImage m = _originalImage.MIplImage;
                byte* dataPtrOriginal = (byte*)m.imageData.ToPointer();
                byte* originalPtr = dataPtrOriginal;

                int width = _originalImage.Width;
                int height = _originalImage.Height;
                int nChan = m.nChannels;
                int padding = m.widthStep - m.nChannels * m.width;
                int step = m.widthStep;


                bool changed = true;
                uint curr_label = 1;

                _backgroundB = dataPtrOriginal[0];
                _backgroundG = dataPtrOriginal[1];
                _backgroundR = dataPtrOriginal[2];

                // First run
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (_backgroundB != dataPtrOriginal[0] || _backgroundG != dataPtrOriginal[1] || _backgroundR != dataPtrOriginal[2])
                        {
                            _labels[x, y] = curr_label++;
                        }
                        else
                        {
                            _labels[x, y] = BASE_VALUE;
                        }
                        dataPtrOriginal += nChan;
                    }
                    dataPtrOriginal += padding;
                }

                // Cycle
                while (changed)
                {
                    changed = false;

                    // Up->Down Left->Right
                    dataPtrOriginal = originalPtr;
                    dataPtrOriginal += nChan + step;
                    for (int y = 1; y < height - 1; y++)
                    {

                        for (int x = 1; x < width - 1; x++)
                        {
                            uint tmp = _labels[x, y];
                            if (_labels[x, y] != BASE_VALUE)
                            {
                                if (_labels[x, y] > _labels[x - 1, y - 1])
                                {
                                    _labels[x, y] = _labels[x - 1, y - 1];
                                }
                                if (_labels[x, y] > _labels[x, y - 1])
                                {
                                    _labels[x, y] = _labels[x, y - 1];
                                }
                                if (_labels[x, y] > _labels[x - 1, y])
                                {
                                    _labels[x, y] = _labels[x - 1, y];
                                }
                                if (_labels[x, y] > _labels[x + 1, y - 1])
                                {
                                    _labels[x, y] = _labels[x + 1, y - 1];
                                }
                                if (_labels[x, y] > _labels[x - 1, y + 1])
                                {
                                    _labels[x, y] = _labels[x - 1, y + 1];
                                }
                                if (_labels[x, y] > _labels[x + 1, y])
                                {
                                    _labels[x, y] = _labels[x + 1, y];
                                }
                                if (_labels[x, y] > _labels[x, y + 1])
                                {
                                    _labels[x, y] = _labels[x, y + 1];
                                }
                                if (_labels[x, y] > _labels[x + 1, y + 1])
                                {
                                    _labels[x, y] = _labels[x + 1, y + 1];
                                }
                                if (tmp != _labels[x, y])
                                    changed = true;
                            }
                            dataPtrOriginal += nChan;
                        }
                        dataPtrOriginal += nChan * 2 + padding;
                    }

                    // Down->Up Right->Left
                    dataPtrOriginal = originalPtr + (nChan * width) + (step * height);
                    dataPtrOriginal -= nChan + step;
                    for (int y = height - 2; y > 0; y--)
                    {
                        for (int x = width - 2; x > 0; x--)
                        {

                            uint tmp = _labels[x, y];
                            if (_labels[x, y] != BASE_VALUE)
                            {
                                if (_labels[x, y] > _labels[x - 1, y - 1])
                                {
                                    _labels[x, y] = _labels[x - 1, y - 1];
                                }
                                if (_labels[x, y] > _labels[x, y - 1])
                                {
                                    _labels[x, y] = _labels[x, y - 1];
                                }
                                if (_labels[x, y] > _labels[x - 1, y])
                                {
                                    _labels[x, y] = _labels[x - 1, y];
                                }
                                if (_labels[x, y] > _labels[x + 1, y - 1])
                                {
                                    _labels[x, y] = _labels[x + 1, y - 1];
                                }
                                if (_labels[x, y] > _labels[x - 1, y + 1])
                                {
                                    _labels[x, y] = _labels[x - 1, y + 1];
                                }
                                if (_labels[x, y] > _labels[x + 1, y])
                                {
                                    _labels[x, y] = _labels[x + 1, y];
                                }
                                if (_labels[x, y] > _labels[x, y + 1])
                                {
                                    _labels[x, y] = _labels[x, y + 1];
                                }
                                if (_labels[x, y] > _labels[x + 1, y + 1])
                                {
                                    _labels[x, y] = _labels[x + 1, y + 1];
                                }
                                if (tmp != _labels[x, y])
                                    changed = true;

                            }
                            dataPtrOriginal -= nChan;
                        }
                        dataPtrOriginal -= nChan * 2 + padding;
                    }
                }
            }

        }

        private int GetLabelIndex(uint label)
        {
            var index = Array.IndexOf(_labelsIndex, label);
            
            if (index != -1) return index;
            
            _labelsIndex[_currLabelIndex] = label;
            index = _currLabelIndex++;
            
            return index;
        }

        private unsafe Image<Bgr, byte> GetImagesPieces(int[] piece, double angle, int helperX, int helperY)
        {
            unsafe
            {
                var original = _originalImage.MIplImage;
                var dataPtrOriginal = (byte*)original.imageData.ToPointer();
                var nChan = original.nChannels;
                var step = original.widthStep;

                int cols, rows;
                
                if (angle.Equals(0))
                {
                    cols = piece[2] - piece[0] + 1;
                    rows = piece[3] - piece[1] + 1;
                }
                else
                {
                    cols = (int)Math.Round(Math.Sqrt(Math.Pow(helperX - piece[0], 2) + Math.Pow(piece[1] - helperY, 2))) + 1;
                    rows = (int)Math.Round(Math.Sqrt(Math.Pow(piece[2] - helperX, 2) + Math.Pow(helperY - piece[3], 2))) + 1;
                }

                var newImage = new Image<Bgr, byte>(cols, rows);
                var m = newImage.MIplImage;
                var newImagePointer = (byte*) m.imageData.ToPointer();
                var nChanNew = m.nChannels;
                var paddingNew = m.widthStep - m.nChannels * m.width;

                var xP = piece[0];
                var yP = piece[1];

                byte*  prevPixel = null;

                for (var y = 0; y < rows; y++)
                {
                    for (var x = 0; x < cols; x++)
                    {
                        int x2;
                        int y2;
                        if (angle.Equals(0))
                        {
                            x2 = x + xP;
                            y2 = y + yP;
                        }
                        else
                        {
                            x2 = (int)Math.Round(x * Math.Cos(-angle) - y * Math.Sin(-angle) + xP);
                            y2 = (int)Math.Round(x * Math.Sin(-angle) + y * Math.Cos(-angle) + yP);
                        }

                        newImagePointer[0] = (dataPtrOriginal + x2 * nChan + y2 * step)[0];
                        newImagePointer[1] = (dataPtrOriginal + x2 * nChan + y2 * step)[1];
                        newImagePointer[2] = (dataPtrOriginal + x2 * nChan + y2 * step)[2];

                        
                        int i = 1;
                        if(newImagePointer[0] == _backgroundB && newImagePointer[1] == _backgroundG && newImagePointer[2] == _backgroundR)
                        {
                            if (prevPixel == null) ;//Console.WriteLine("ERROR null prev pixel");
                            else
                            {
                                newImagePointer[0] = prevPixel[0];
                                newImagePointer[1] = prevPixel[1];
                                newImagePointer[2] = prevPixel[2];
                                i++;
                                //Console.WriteLine("III: " + i);
                            }
                        }
                        else
                        {
                            prevPixel = (dataPtrOriginal + x2 * nChan + y2 * step);
                        }

                        

                        newImagePointer += nChanNew;
                    }
                    
                    newImagePointer += paddingNew;
                }

               // newImage.Save("./imgs/piece" + partsID++ + ".png");

                return newImage;
            }
        }

        public void GetPiecesPosition(out List<int[]> Pieces_positions, out List<int> Pieces_angle)
        {
            var width = _labels.GetLength(0);
            var height = _labels.GetLength(1);

            Pieces_positions = new List<int[]>();
            Pieces_angle = new List<int>();

            var xTopLeft = new int[MaxNumberPieces];
            var yTopLeft = new int[MaxNumberPieces];
            
            var xHelper = new int[MaxNumberPieces];
            var yHelper = new int[MaxNumberPieces];
            
            var xBottomRight = new int[MaxNumberPieces];
            var yBottomRight = new int[MaxNumberPieces];

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (_labels[x, y] == BASE_VALUE) continue;
                    
                    // TODO if a value is already set ignore / overwrite depending
                    
                    if (_labels[x, y - 1] == BASE_VALUE && _labels[x - 1, y] == BASE_VALUE && _labels[x - 1, y + 2] == BASE_VALUE)
                    {
                        // Found a top left corner
                        var index = GetLabelIndex(_labels[x, y]);

                        xTopLeft[index] = x;
                        yTopLeft[index] = y;

                        //Console.WriteLine("Label: " + _labels[x, y]);
                        //Console.WriteLine("Top Left X: " + x);
                        //Console.WriteLine("Top Left Y: " + y);

                    }
                    else if (_labels[x, y + 1] == BASE_VALUE && _labels[x + 1, y] == BASE_VALUE && _labels[x + 1, y - 2] == BASE_VALUE)
                    {
                        // Found a bottom right corner
                        var index = GetLabelIndex(_labels[x, y]);
                        
                        xBottomRight[index] = x;
                        yBottomRight[index] = y;

                        //Console.WriteLine("Label: " + _labels[x, y]);
                        //Console.WriteLine("Bottom Right X: " + x);
                        //Console.WriteLine("Bottom Right Y: " + y);
                    }
                    else if (_labels[x, y - 1] == BASE_VALUE && _labels[x + 1, y] == BASE_VALUE && _labels[x - 2, y - 1] == BASE_VALUE)
                    {
                        // Found a top right corner (helper)
                        var index = GetLabelIndex(_labels[x, y]);

                        xHelper[index] = x;
                        yHelper[index] = y;

                        //Console.WriteLine("Label: " + _labels[x, y]);
                        //Console.WriteLine("Top Right X: " + x);
                        //Console.WriteLine("Top Right Y: " + y);
                    }
                }
            }

            for (var i = 0; i < _currLabelIndex; i++)
            {
                var pieceVector = new int[4];
                pieceVector[0] = xTopLeft[i];   // x- Top-Left 
                pieceVector[1] = yTopLeft[i];  // y- Top-Left
                pieceVector[2] = xBottomRight[i]; // x- Bottom-Right
                pieceVector[3] = yBottomRight[i]; // y- Bottom-Right
                
                Pieces_positions.Add(pieceVector);
                double rads;
                
                if (xHelper[i] == xTopLeft[i] && yHelper[i] == yTopLeft[i])
                {
                    // No rotation needed
                    Pieces_angle.Add(0);
                    rads = 0.0;
                }
                else
                {
                    //Calculate Angle
                    double opposite = yTopLeft[i] - yHelper[i];
                    double adjacent = xHelper[i] - xTopLeft[i];
                    
                    rads = Math.Tanh(opposite / adjacent);
                    //Console.WriteLine("Angle:\t" + RadsToDegrees(rads));
                    
                    Pieces_angle.Add(RadsToDegrees(rads));
                }

                _imagesPieces.Add(GetImagesPieces(pieceVector, rads, xHelper[i], yHelper[i]));
            }
        }

        private int[] FindBestStoredValueIndexes(SideValues[,] bestDiffs)
        {
            var topPoints = 0.0;
            int bestI = -1, bestJ = -1;

            for (var i = 0; i < _imagesPieces.Count; i++)
            {
                for (var j = 0; j < _imagesPieces.Count; j++)
                {
                    // Only check half of the matrix
                    if (i <= j) continue;

                    if (bestDiffs[i, j].Points <= topPoints) continue;

                    topPoints = bestDiffs[i, j].Points;
                    bestI = i;
                    bestJ = j;
                }
            }

            int[] res = { bestI, bestJ };
            return res;
        }

        private SideValues[,] CopyMatrix(SideValues[,] bestDiffs, int iToRemove, int jToRemove)
        {
            // Create new copied matrix without specified elements
            SideValues[,] toReturn = new SideValues[bestDiffs.GetLength(0) - 1, bestDiffs.GetLength(0) - 1];

            for(int oldI = 0, newI = 0; oldI < bestDiffs.GetLength(0); oldI++)
            {
                if (oldI == iToRemove || oldI == jToRemove)
                    continue;

                for (int oldJ = 0, newJ = 0; oldJ < bestDiffs.GetLength(1); oldJ++)
                {
                    if (oldJ == jToRemove || oldJ == iToRemove)
                        continue;

                    toReturn[newI, newJ] = bestDiffs[oldI, oldJ];
                    newJ++;
                }
                newI++;
            }

            Image<Bgr, byte> newImage = _imagesPieces[_imagesPieces.Count - 1];

            int pieceIndex = 0;
            foreach (var piece in _imagesPieces)
            {
                if (pieceIndex != _imagesPieces.Count - 1)
                {
                    toReturn[bestDiffs.GetLength(0) - 2, pieceIndex++] = PuzzleHelper.CompareSides(newImage, piece);
                }               
            }

            return toReturn;
        }

        private Image<Bgr, byte> CombinePieces(SideValues[,] bestDiffs, int bestPiece1, int bestPiece2)
        {

            int bestSide = bestDiffs[bestPiece1, bestPiece2].Side;

            /*//Console.WriteLine("Piece1:\t" + bestPiece1);
            //Console.WriteLine("Piece2:\t" + bestPiece2);
            //Console.WriteLine("Side:\t" + bestSide);*/
            
            var piece1 = _imagesPieces[bestPiece1];
            var piece2 = _imagesPieces[bestPiece2];



            // Combine Pieces
            Image<Bgr, byte> newPiece;

            switch (bestSide)
            {
                case 0:
                    newPiece = PuzzleHelper.CombinePiecesTopBottom(piece1, piece2);
                    break;
                case 1:
                    newPiece = PuzzleHelper.CombinePiecesRightLeft(piece1, piece2);
                    break;
                case 2:
                    newPiece = PuzzleHelper.CombinePiecesBottomTop(piece1, piece2);
                    break;
                case 3:
                    newPiece = PuzzleHelper.CombinePiecesLeftRight(piece1, piece2);
                    break;
                default:
                    ////Console.WriteLine("ERROR: Combining pieces");
                    newPiece = new Image<Bgr, byte>(0,0);
                    break;
            }

           // newPiece.Save("./imgs/part" + pieceID++ + ".png");

            return newPiece;
        }
        
        public Image<Bgr, byte> GetFinalImage()
        {
            // TODO Keep values between iterations
            SideValues[,] bestDiffs = new SideValues[_imagesPieces.Count, _imagesPieces.Count];

            int bestI = 0, bestJ = 0;

            // Fill comparing matrix with initial values
            var currPos = 0;
            foreach (var currPiece in _imagesPieces)
            {
                var nextPos = 0;
                foreach (var nextPiece in _imagesPieces)
                {
                    if (currPos > nextPos)
                    {
                        // ////Console.WriteLine("Best Side: " + currPos + " vs: " + nextPos);

                        bestDiffs[currPos, nextPos] = PuzzleHelper.CompareSides(currPiece, nextPiece);

                        if (bestDiffs[currPos, nextPos].Points > bestDiffs[bestI, bestJ].Points)
                        {
                            bestI = currPos;
                            bestJ = nextPos;
                        }
                    }

                    nextPos++;
                }

                currPos++;
            }

            while (_imagesPieces.Count != 1)
            {
                var piece1 = _imagesPieces[bestI];
                var piece2 = _imagesPieces[bestJ];

                Image<Bgr, byte> newPiece = CombinePieces(bestDiffs, bestI, bestJ);

                _imagesPieces.Remove(piece1);
                _imagesPieces.Remove(piece2);

                _imagesPieces.Add(newPiece);

                ////Console.WriteLine("MATRIX BEFORE//////////////////////////");
                for (int i = 0; i < bestDiffs.GetLength(0); i++)
                {
                    for (int j = 0; j < bestDiffs.GetLength(1); j++)
                    {
                        ////Console.Write(bestDiffs[i, j].Points + "\t");
                    }
                    //Console.WriteLine("");
                }
                //Console.WriteLine("END //////////////////////////");

                bestDiffs = CopyMatrix(bestDiffs, bestI, bestJ);

                //Console.WriteLine("MATRIX //////////////////////////");
                for(int i = 0; i < bestDiffs.GetLength(0); i++)
                {
                    for(int j = 0; j < bestDiffs.GetLength(1); j++)
                    {
                        //Console.Write(bestDiffs[i, j].Points + "\t");
                    }
                    //Console.WriteLine("");
                }
                //Console.WriteLine("END //////////////////////////");

                int[] res = FindBestStoredValueIndexes(bestDiffs);

                bestI = res[0];
                bestJ = res[1];
            }

            return _imagesPieces[0];
        }

    }

}
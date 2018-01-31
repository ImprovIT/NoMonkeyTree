using System.Collections.Generic;

namespace ITI.GameOfLife
{
    public class Game
    {
        HashSet<Coordinate> _aliveCells;

        public Game()
        {
            _aliveCells = new HashSet<Coordinate>();
        }

        public void GiveLive( int x, int y )
        {
            _aliveCells.Add( new Coordinate( x, y ) );
        }

        public bool IsAlive( int x, int y )
        {
            return _aliveCells.Contains( new Coordinate( x, y ) );
        }

        public void Kill( int x, int y )
        {
            _aliveCells.Remove( new Coordinate( x, y ) );
        }

        public bool NextTurn()
        {
            HashSet<Coordinate> aliveCells = new HashSet<Coordinate>();
            bool stateChanged = false;
            foreach( Coordinate c in GetCellsToCheck() )
            {
                bool nextState = GetNextState( c );
                if( nextState ) aliveCells.Add( c );
                if( nextState != IsAlive( c.X, c.Y ) ) stateChanged = true;
            }
            _aliveCells = aliveCells;
            return stateChanged;
        }

        bool GetNextState( Coordinate c )
        {
            int aliveNeighbors = CountAliveNeighbors( c );
            return aliveNeighbors == 3
                || IsAlive( c.X, c.Y ) && aliveNeighbors == 2;
        }

        int CountAliveNeighbors( Coordinate c )
        {
            int count = 0;
            for(int i = -1; i < 2; i++ )
            {
                for(int j = -1; j < 2; j++ )
                {
                    if( ( i != 0 || j != 0 ) && IsAlive( c.X + i, c.Y + j ) ) count++;
                }
            }
            return count;
        }

        HashSet<Coordinate> GetCellsToCheck()
        {
            HashSet<Coordinate> cellsToCheck = new HashSet<Coordinate>();
            foreach( Coordinate cell in _aliveCells )
            {
                for( int i = -1; i < 2; i++ )
                {
                    for( int j = -1; j < 2; j++ ) cellsToCheck.Add( new Coordinate( cell.X + i, cell.Y + j ) );
                }
            }
            return cellsToCheck;
        }

        struct Coordinate
        {
            internal Coordinate( int x, int y )
            {
                X = x;
                Y = y;
            }

            internal int X { get; }

            internal int Y { get; }
        }
    }
}

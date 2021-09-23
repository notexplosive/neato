using System;
using System.Collections.Generic;

namespace Neato
{
    public class TokenList
    {
        private readonly List<string> list;
        private int currentPosition = 0;

        public TokenList(string[] args)
        {
            this.list = new List<string>();
            foreach (var arg in args)
            {
                this.list.Add(arg);
            }
        }

        public string NextString()
        {
            if (this.list.Count > 0)
            {
                var item = this.list[0];
                this.list.RemoveAt(0);
                this.currentPosition++;

                return item;
            }
            else
            {
                throw new TokenizerFailedException(string.Format("Missing value at position {0}", this.currentPosition));
            }
        }

        public int NextInt()
        {
            var token = NextString();
            var canParse = int.TryParse(token, out int result);

            if (canParse)
            {
                return result;
            }
            else
            {
                throw new TokenizerFailedException(string.Format("Expected integer at position {0}, got {1}", this.currentPosition, token.Length > 0 ? token : "nothing"));
            }
        }
    }

    public class TokenizerFailedException : Exception
    {
        public TokenizerFailedException(string message) : base(message)
        {
        }
    }
}

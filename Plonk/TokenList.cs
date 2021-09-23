using System;
using System.Collections.Generic;

namespace Neato
{
    public class TokenList
    {
        private readonly List<string> list;
        private int currentPosition = 0;
        private string error = string.Empty;
        private bool hasError;

        public string Error()
        {
            return this.error;
        }

        public void SaveError(string text)
        {
            if (this.error.Length > 0)
            {
                this.error += "\n";
            }
            this.error += text;
            this.hasError = true;
        }

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
                SaveError(string.Format("Missing value at position {0}", this.currentPosition));
                throw new TokenizerFailedException();
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
                SaveError(string.Format("Expected integer at position {0}, got {1}", this.currentPosition, token.Length > 0 ? token : "nothing"));
                throw new TokenizerFailedException();
            }
        }

        public bool HasFailure()
        {
            return this.hasError;
        }
    }

    public class TokenizerFailedException : Exception
    {
    }
}

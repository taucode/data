using System;
using System.Collections.Generic;
using TauCode.Data.Exceptions;

namespace TauCode.Data.EmojiSupport
{
    // todo: clean, regions, rearrange
    internal class EmojiNode
    {
        private Emoji? _emoji;
        private readonly Dictionary<char, EmojiNode> _followers;

        internal EmojiNode(char? c)
        {
            this.Char = c;
            _followers = new Dictionary<char, EmojiNode>();
        }

        internal char? Char { get; }

        internal Emoji? Emoji
        {
            get => _emoji;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_emoji != null)
                {
                    throw new InvalidOperationException("Emoji already set.");
                }

                _emoji = value;
            }
        }

        internal IReadOnlyDictionary<char, EmojiNode> Followers => _followers;

        internal void AddEmoji(Emoji emoji)
        {
            this.CheckIsRoot();
            this.AddEmojiTail(emoji, 0);
        }

        internal int? TryExtract(ReadOnlySpan<char> input, out Emoji? emoji, out TextDataExtractionException error)
        {
            if (this.Char != null)
            {
                throw new InvalidOperationException("Only applicable to the root node.");
            }

            if (input.Length == 0)
            {
                emoji = null;
                error = Helper.CreateException(ExtractionError.EmptyInput, null);
                return null;
            }

            Emoji? lastEmoji = null;
            int? lastSuccessfulOffset = null;

            var offset = 0;
            var current = this;

            while (true)
            {
                if (offset == input.Length)
                {
                    if (lastEmoji == null)
                    {
                        emoji = null;
                        error = Helper.CreateException(ExtractionError.UnexpectedEnd, offset);
                        return null;
                    }
                    else
                    {
                        emoji = lastEmoji.Value;
                        error = null;
                        return lastSuccessfulOffset.Value + 1;
                    }
                }

                var c = input[offset];

                var follower = current._followers.GetValueOrDefault(c);

                if (follower == null)
                {
                    if (current.Emoji.HasValue)
                    {
                        emoji = current.Emoji.Value;
                        error = null;
                        return offset; // todo: error? should be 'offset + 1'?
                    }
                    else
                    {
                        if (lastEmoji == null)
                        {
                            emoji = null;
                            error = Helper.CreateException(ExtractionError.NonEmojiChar, offset); // todo: sure?
                            return null;
                        }
                        else
                        {
                            emoji = lastEmoji.Value;
                            error = null;
                            return lastSuccessfulOffset.Value + 1;
                        }
                    }
                }
                else
                {
                    if (follower.Emoji.HasValue)
                    {
                        lastEmoji = follower.Emoji.Value;
                        lastSuccessfulOffset = offset;
                    }

                    offset++;

                    if (offset == input.Length)
                    {
                        if (lastEmoji == null)
                        {
                            emoji = null;
                            error = Helper.CreateException(ExtractionError.UnexpectedEnd, offset);
                            return null;
                        }
                        else
                        {
                            emoji = lastEmoji.Value;
                            error = null;
                            return lastSuccessfulOffset.Value + 1;
                        }
                    }

                    current = follower;
                }
            }
        }

        private void AddEmojiTail(Emoji emoji, int offset)
        {
            var c = emoji.Value[offset];
            var follower = this.GetOrCreateFollower(c);

            if (offset == emoji.Value.Length - 1)
            {
                follower.Emoji = emoji;
            }
            else
            {
                follower.AddEmojiTail(emoji, offset + 1);
            }
        }

        private EmojiNode GetOrCreateFollower(char c)
        {
            var follower = _followers.GetValueOrDefault(c);
            if (follower == null)
            {
                follower = new EmojiNode(c);
                _followers.Add(c, follower);
            }

            return follower;
        }

        internal bool HasPath(ReadOnlySpan<char> path, int length)
        {
            if (this.Char != null)
            {
                throw new InvalidOperationException("Only applicable to the root node.");
            }

            var cleanPath = path[..Math.Min(path.Length, length)];

            return this.HasPathInternal(cleanPath);
        }

        private bool HasPathInternal(ReadOnlySpan<char> cleanPath)
        {
            if (cleanPath.Length == 0)
            {
                return true;
            }

            var follower = _followers.GetValueOrDefault(cleanPath[0]);
            if (follower == null)
            {
                return false;
            }

            return follower.HasPathInternal(cleanPath[1..]);
        }

        internal int Skip(ReadOnlySpan<char> input, out ExtractionError? error)
        {
            if (this.Char != null)
            {
                throw new InvalidOperationException("Only applicable to the root node.");
            }

            if (input.Length == 0)
            {
                error = ExtractionError.EmptyInput;
                return 0;
            }

            Emoji? lastEmoji = null;
            var lastSuccessfulOffset = 0;

            var offset = 0;
            var current = this;

            while (true)
            {
                if (offset == input.Length)
                {
                    if (lastEmoji == null)
                    {
                        error = ExtractionError.IncompleteEmoji;
                        return offset;
                    }
                    else
                    {
                        error = null;
                        return lastSuccessfulOffset + 1;
                    }
                }

                var c = input[offset];

                var follower = current._followers.GetValueOrDefault(c);

                if (follower == null)
                {
                    if (current.Emoji.HasValue)
                    {
                        error = null;
                        return offset;
                    }
                    else
                    {
                        if (lastEmoji == null)
                        {
                            error = ExtractionError.NonEmojiChar;
                            return offset;
                        }
                        else
                        {
                            error = null;
                            return lastSuccessfulOffset + 1;
                        }
                    }
                }
                else
                {
                    if (follower.Emoji.HasValue)
                    {
                        lastEmoji = follower.Emoji.Value;
                        lastSuccessfulOffset = offset;
                    }

                    offset++;
                    current = follower;
                }
            }
        }

        private void CheckIsRoot()
        {
            if (this.Char != null)
            {
                throw new InvalidOperationException("Only applies to the root node."); // should never happen
            }
        }
    }
}

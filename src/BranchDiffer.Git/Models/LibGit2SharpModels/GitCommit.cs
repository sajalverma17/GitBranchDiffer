﻿using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchDiffer.Git.Models.LibGit2SharpModels
{
    public interface IGitCommit
    {
        string Sha { get; }

        Tree Tree { get; }
    }

    public class GitCommit : IGitCommit
    {
        private readonly Commit commit;

        public GitCommit(Commit commit)
        {
            this.commit = commit;
        }

        public string Sha => this.commit.Sha;

        public Tree Tree => this.commit.Tree;

        /// <summary>
        /// Compare git commit objects by their 40-char sha1
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is IGitCommit otherCommit))
            {
                throw new InvalidOperationException();
            }

            return this.Sha.Equals(otherCommit.Sha);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
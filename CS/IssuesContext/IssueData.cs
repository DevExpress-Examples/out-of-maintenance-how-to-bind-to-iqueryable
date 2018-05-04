using System;
using System.Linq;

namespace InfiniteAsyncSourceEFSample {
    public class IssueData {
        public static IQueryable<IssueData> Select(IQueryable<Issue> issues) {
            return issues.Select(x => new IssueData() {
                Id = x.Id,
                Subject = x.Subject,
                User = x.User.FirstName + " " + x.User.LastName,
                Created = x.Created,
                Votes = x.Votes,
                Priority = x.Priority,
            });
        }
        public int Id { get; private set; }
        public string Subject { get; private set; }
        public DateTime Created { get; private set; }
        public int Votes { get; private set; }
        public Priority Priority { get; private set; }

        public string User { get; private set; }
    }
}

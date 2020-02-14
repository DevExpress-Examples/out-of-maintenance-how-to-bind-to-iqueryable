using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;

namespace InfiniteAsyncSourceEFSample {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            var source = new InfiniteAsyncSource() {
                ElementType = typeof(IssueData)
            };
            Unloaded += (o, e) => {
                source.Dispose();
            };

            source.FetchRows += (o, e) => {
                e.Result = Task.Run(() => FetchRows(e));
            };

            source.GetUniqueValues += (o, e) => {
                if (e.PropertyName == "User") {
                    e.ResultWithCounts = Task.Run(() => GetIssueDataQueryable().DistinctWithCounts(e.PropertyName));
                }
                e.Result = Task.Run(() => GetIssueDataQueryable().Distinct(e.PropertyName));
            };

            source.GetTotalSummaries += (o, e) => {
                e.Result = Task.Run(() => {
                    var queryable = GetIssueDataQueryable()
                        .Where(MakeFilterExpression(e.Filter));
                    return queryable.GetSummaries(e.Summaries);
                });
            };

            grid.ItemsSource = source;
        }
        static FetchRowsResult FetchRows(FetchRowsAsyncEventArgs e) {
            var queryable = GetIssueDataQueryable()
                .SortBy(e.SortOrder, defaultUniqueSortPropertyName: "Id")
                .Where(MakeFilterExpression(e.Filter));

            return queryable
                .Skip(e.Skip)
                .Take(30)
                .ToArray();
        }

        void OnSearchStringToFilterCriteria(object sender, SearchStringToFilterCriteriaEventArgs e) {
            e.Filter = new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty("Subject"), e.SearchString);
        }

        static Expression<Func<IssueData, bool>> MakeFilterExpression(CriteriaOperator filter) {
            var converter = new GridFilterCriteriaToExpressionConverter<IssueData>();
            converter.RegisterFunctionExpressionFactory(
                operatorType: FunctionOperatorType.StartsWith, 
                factory: (string value) => {
                    var toLowerValue = value.ToLower();
                    return x => x.ToLower().StartsWith(toLowerValue);
                });
            return converter.Convert(filter);
        }

        static IQueryable<IssueData> GetIssueDataQueryable() {
            var context = new IssuesContext();
            return IssueData.Select(context.Issues);
        }
    }
}

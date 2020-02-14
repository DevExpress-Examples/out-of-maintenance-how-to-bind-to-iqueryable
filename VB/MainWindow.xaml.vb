Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports DevExpress.Xpf.Grid
Imports System
Imports System.Data.Entity
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Threading.Tasks
Imports System.Windows

Namespace InfiniteAsyncSourceEFSample
	Partial Public Class MainWindow
		Inherits Window

		Public Sub New()
			InitializeComponent()
			Dim source = New InfiniteAsyncSource() With {.ElementType = GetType(IssueData)}
			AddHandler Me.Unloaded, Sub(o, e)
				source.Dispose()
			End Sub

			AddHandler source.FetchRows, Sub(o, e)
				e.Result = Task.Run(Function() FetchRows(e))
			End Sub

			AddHandler source.GetUniqueValues, Sub(o, e)
				If e.PropertyName = "User" Then
					e.ResultWithCounts = Task.Run(Function() GetIssueDataQueryable().DistinctWithCounts(e.PropertyName))
				End If
				e.Result = Task.Run(Function() GetIssueDataQueryable().Distinct(e.PropertyName))
			End Sub

			AddHandler source.GetTotalSummaries, Sub(o, e)
				e.Result = Task.Run(Function()
					Dim queryable = GetIssueDataQueryable().Where(MakeFilterExpression(e.Filter))
					Return queryable.GetSummaries(e.Summaries)
				End Function)
			End Sub

			grid.ItemsSource = source
		End Sub
		Private Shared Function FetchRows(ByVal e As FetchRowsAsyncEventArgs) As FetchRowsResult
			Dim queryable = GetIssueDataQueryable().SortBy(e.SortOrder, defaultUniqueSortPropertyName:= "Id").Where(MakeFilterExpression(e.Filter))

			Return queryable.Skip(e.Skip).Take(30).ToArray()
		End Function

		Private Sub OnSearchStringToFilterCriteria(ByVal sender As Object, ByVal e As SearchStringToFilterCriteriaEventArgs)
			e.Filter = New FunctionOperator(FunctionOperatorType.StartsWith, New OperandProperty("Subject"), e.SearchString)
		End Sub

		Private Shared Function MakeFilterExpression(ByVal filter As CriteriaOperator) As Expression(Of Func(Of IssueData, Boolean))
			Dim converter = New GridFilterCriteriaToExpressionConverter(Of IssueData)()
			converter.RegisterFunctionExpressionFactory(operatorType:= FunctionOperatorType.StartsWith, factory:= Function(value As String)
				Dim toLowerValue = value.ToLower()
				Return x
				If True Then
					Get
						Return x.ToLower().StartsWith(toLowerValue)
					End Get
				End If
			End Function)
			Return converter.Convert(filter)
		End Function

		Private Shared Function GetIssueDataQueryable() As IQueryable(Of IssueData)
			Dim context = New IssuesContext()
			Return IssueData.Select(context.Issues)
		End Function
	End Class
End Namespace

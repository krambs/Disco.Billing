var BillingSummary = React.createClass({
    getInitialState: function() {
        return { data: [] };
    },
    componentDidMount: function() {
        $.ajax({
            url: "home/BillingData",
            dataType: "json",
            cache: false,
            success: function(data) {
                this.setState({ data: data });
            }.bind(this)
        });
    },
    render: function () {
        let accountSummaries = [];

        return (
            <div>
                <h1>Billing Summary</h1>
                {
                    this.state.data.map(
                        function(item) {
                             return <BillingSummary.AccountSummary />;
                        }
                    )
                }
            </div>
        );
    }
});

BillingSummary.AccountSummary = props => (
    <div>
        Gabe Rules
    </div>
);

ReactDOM.render(
    <BillingSummary />,
    document.getElementById("content")
);
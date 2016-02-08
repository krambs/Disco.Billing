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
        return (
            <div>
                <h1>Billing Summary</h1>
                {
                    this.state.data.map(
                        function(billingAccountContractGroup) {
                            return <BillingSummary.AccountSummary 
                                    key={billingAccountContractGroup.BillingAccountId}
                                    billingAccountName={billingAccountContractGroup.BillingAccountName}
                                    contracts={billingAccountContractGroup.Contracts} />;
                        }
                    )
                }
            </div>
        );
    }
});

BillingSummary.AccountSummary = props => (
    <div>
        <h3>{props.billingAccountName}</h3>
        <h4>{props.contracts.reduce((previousValue, currentValue) => previousValue + currentValue.Matters.length, 0)}</h4>
    </div>
);

ReactDOM.render(
    <BillingSummary />,
    document.getElementById("content")
);
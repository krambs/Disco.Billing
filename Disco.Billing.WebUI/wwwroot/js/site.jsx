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
                <table className="table table-sm">
                    <thead></thead>
                    <tbody>
                        {
                            this.state.data.map(
                                function(billingAccountContractGroup) {
                                    return <BillingSummary.AccountRow key={billingAccountContractGroup.BillingAccountId}
                                                       billingAccountName={billingAccountContractGroup.BillingAccountName}
                                                       contracts={billingAccountContractGroup.Contracts} />;
                                }
                            )
                        }
                    </tbody>
                </table>
                
            </div>
        );
    }
});

BillingSummary.AccountRow = props => (
    <tr>
        <td>{props.billingAccountName}</td>
        <td>{
                currencyFormatter.format(
                    props.contracts.reduce(
                        function (previous, current) {
                            return previous + calculateContractBillingOnDate(current, moment.utc("2016-01-01"));
                        },
                        0
                    )
                )
            }</td>
    </tr>
);

ReactDOM.render(
    <BillingSummary />,
    document.getElementById("content")
);
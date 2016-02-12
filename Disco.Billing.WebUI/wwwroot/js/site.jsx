var BillingSummary = React.createClass({
    getInitialState: function() {
        return {
            data: [],
            invoiceDate: moment.utc("2016-02-01")
        };
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
        const invoiceDate = this.state.invoiceDate;

        return (
            <div>
                
                {
                    this.state.data.map(
                        function(billingAccountContractGroup) {
                            return <BillingSummary.InvoiceTable 
                                    key={billingAccountContractGroup.BillingAccountId}
                                    billingAccountName={billingAccountContractGroup.BillingAccountName}
                                    contracts={billingAccountContractGroup.Contracts} 
                                    invoiceDate={invoiceDate} 
                                    oneMonthBeforeInvoiceDate={invoiceDate.add(-1, "month")} />;
                        }
                    )
                }
                
            </div>
        );
    }
});

BillingSummary.InvoiceTable = props => (
    <div className="invoice">
        <div className="invoice-header clearfix">
            <h2>{props.billingAccountName}</h2>
            <div className="invoice-header-stamp">
                INVOICE
            </div>
        </div>
        {
            props.contracts.map(
                contract => contract.Matters.map(
                    matter => <BillingSummary.MatterSection 
                                key={matter.Name} 
                                matter={matter} 
                                invoiceDate={props.invoiceDate} 
                                oneMonthBeforeInvoiceDate={props.oneMonthBeforeInvoiceDate} />
                )
            )
        }
    </div>
);

BillingSummary.MatterSection = props => {
    var dataSetEntries = [];
    props.matter.DataSets
        .filter(dataSet => dataSet.DataSize > 0)
        .forEach(dataSet => {
            dataSetEntries.push(<BillingSummary.DataSetEntry key={dataSet.CreatedDate} createdDate={dataSet.CreatedDate} dataSize={dataSet.DataSize} />);
        }
    );

    return(
        <div>
            <h3>{props.matter.Name}</h3>
            <div>Existing data: 
                {
                    props.matter.DataSets
                        .filter(dataSet => moment.utc(dataSet.CreatedDate).isBefore(props.oneMonthBeforeInvoiceDate))
                        .reduce((previous, current) => previous + current.DataSize, 0)
                }
            </div>
        </div>
    );
};

BillingSummary.DataSetEntry = props => (
    <div>{moment.utc(props.createdDate).format("l")} {props.dataSize}</div>
);

BillingSummary.AccountRow = props => (
    <tr>
        <td>{props.billingAccountName}</td>
        <td>{
                currencyFormatter.format(
                    props.contracts.reduce(
                        function (previous, current) {
                            return previous + calculateContractBillingOnDate(current, moment.utc("2016-02-01"));
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
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
                                    oneMonthBeforeInvoiceDate={moment.utc(invoiceDate).add(-1, "month")} />;
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
                <div className="invoice-header-stamp-date">{props.invoiceDate.format("L")}</div>
            </div>
        </div>

        <h3>Matters</h3>
        {
            props.contracts.map(
                contract => contract.Matters.map(
                    matter => <BillingSummary.MatterSection 
                                key={matter.Name} 
                                matter={matter} 
                                contract={contract}
                                invoiceDate={props.invoiceDate} 
                                oneMonthBeforeInvoiceDate={props.oneMonthBeforeInvoiceDate} />
                )
            )
        }
    </div>
);

BillingSummary.MatterSection = props => {
    const existingDataSize = props.matter.DataSets
        .filter(dataSet => moment.utc(dataSet.CreatedDate).isBefore(props.oneMonthBeforeInvoiceDate))
        .reduce((previous, current) => previous + current.DataSize, 0);

    const addedData = _.chain(props.matter.DataSets)
        .filter(dataSet => dataSet.DataSize > 0 && moment.utc(dataSet.CreatedDate).isBefore(props.invoiceDate) && !moment.utc(dataSet.CreatedDate).isBefore(props.oneMonthBeforeInvoiceDate))
        .groupBy(dataSet => moment.utc(dataSet.CreatedDate).format("YYYY-MM-DD"))
        .toPairs()
        .value();

    console.log(addedData)

    const addedDataRows = _.chain(addedData)
        .map(dataSetGroup => <BillingSummary.AddedDataRow key={dataSetGroup[0]} dataSize={_.sumBy(dataSetGroup[1], "DataSize")} createdDate={moment.utc(dataSetGroup[0])} transactionalPricePerGB={props.contract.TransactionalPricePerGB}/>)
        .value();

    let addedDataHeader = null;
    let addedDataTotalHeader = null;
    let addedDataTotal = null;
    if (addedDataRows.length) {
        addedDataHeader = (
            <tr className="header">
                <td></td>
                <td style={{textTransform: "uppercase"}} className="text-md-right">Data added last month</td>
                <td></td>
                <td></td>
                <td className="small text-muted">PRORATED</td>
            </tr>
        );

        addedDataTotalHeader = <BillingSummary.AddedDataTotalHeader />;
        addedDataTotal = <BillingSummary.AddedDataTotal />
    }
    
    return (
        <div className="matter-section">
            <table>
                <tbody>
                    <tr>
                        <td colSpan="4"><h4>{props.matter.Name}</h4></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style={{textTransform: "uppercase"}} className="text-md-right">Existing data</td>
                        <td className="text-md-right">{existingDataSize} GB</td>
                        <td></td>
                        <td></td>
                    </tr>
                    {addedDataHeader}
                    {addedDataRows}
                    {addedDataTotalHeader}
                </tbody>
            </table>
        </div>
    );
};

BillingSummary.AddedDataRow = props => {
    const monthName = props.createdDate.format("MMMM");
    const daysInMonth = moment.utc(props.createdDate).endOf("month").date();
    const daysDataActive = daysInMonth - props.createdDate.date() + 1;
    return(
        <tr>
            <td></td>
            <td className="text-md-right">{props.createdDate.format('l')}</td>
            <td className="text-md-right">{props.dataSize} GB</td>
            <td className="text-md-right text-muted">${props.transactionalPricePerGB} / GB</td>
            <td className="text-md-right">{accounting.formatMoney((daysDataActive / daysInMonth) * props.dataSize * props.transactionalPricePerGB)}</td>
        </tr>
    );
};

BillingSummary.AddedDataTotalHeader = props => {
    return(
        <tr className="header">
            <td></td>
            <td style={{textTransform: "uppercase"}} className="text-md-right font-weight-bold">January Total</td>
            <td></td>
            <td></td>
            <td className="small text-muted">PRORATED</td>
        </tr>
    );
};

BillingSummary.AddedDataTotal = props => {
    return(
        <tr className="header">
            <td></td>
            <td style={{textTransform: "uppercase"}} className="text-md-right font-weight-bold">January Total</td>
            <td></td>
            <td></td>
            <td className="small text-muted">PRORATED</td>
        </tr>
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
var currencyFormatter = Intl.NumberFormat("en-US", { style: "currency", currency: "USD" });

function calculateContractBillingOnDate(contract, date) {
    if (contract.Type === "Transactional") {
        return contract.Matters.reduce(
            function(previousTotal, currentMatter) {
                return previousTotal + calculateMatterBillingOnDate(contract, currentMatter, date);
            },
            0
        );
    } else {
        return 0;
    }
}

function calculateMatterBillingOnDate(contract, matter, date) {
    return matter.DataSets.reduce(
        function(previous, current) {
            return previous + current.DataSize;
        },
        0
    );
}
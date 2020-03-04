CREATE PROCEDURE dbo.SP_InternalSaleDaterange
    @fDate Date = null,
    @toDate Date = null
AS
	SELECT i.PartNumber, i.CustomerName, SUM(ins.Price) as Price, i.InvoiceDiscount,i.MadyBy, CONVERT(DATE, i.InvoiceDate) as [Date], (SUM(ins.Price) - i.InvoiceDiscount) as Total FROM Invoices as i Join InternalSales as ins on i.InvoiceID = ins.InvoiceID WHERE CONVERT(DATE, i.InvoiceDate) BETWEEN @fDate AND @toDate GROUP BY i.PartNumber, i.InvoiceDate, i.InvoiceDiscount, i.MadyBy, i.CustomerName


EXEC dbo.SP_InternalSaleDaterange '2018-07-17','2018-07-23'


SELECT * FROM Invoices 
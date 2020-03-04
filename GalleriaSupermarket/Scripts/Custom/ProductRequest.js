/// <reference path="../jquery-3.3.1.min.js" />

$(document).ready(function () {
    $("#ItemShow").DataTable({
        'aoColumnDefs': [{
            'bSortable': false,
            'aTargets': ['nosort']
        }]
    });
    $('#ReqBtn').on('click', SaveRequest);

    $('.fclose').click(function () {
        $('.detais-box').hide();
    });

    $('#ItemShow').on('click', '.show-details-btn', function () {
        var rowid = $(this).parents('tr').attr('data-req');
        showProductReqDetails(rowid);
    });
    $('#appBtn').on('click', function () {
        var reqid = parseInt($(this).parents('.main-row').find('pre').attr('data-req'));
        ApprovedRequestByAdmin(reqid);
    });

});

GetProductRequest();
function GetProductRequest() {
    $.ajax({
        url: '/Item/GetProductRequest',
        type: 'GET',
        dataType: 'json',
        success: function (res) {
            var html;
            $('#ItemShow tbody').empty();
            var status,sClass;

            var detailsBtn = '<div class="action-buttons"><a href="javascript:void(0)" class="green bigger-140 show-details-btn" title="Show Details"><i class="ace-icon fa fa-th-list"></i><span class="sr-only">Details</span></a></div>';

            $.each(res, function (index, row) {
                if (row.approved === true) {
                    status = 'Granted';
                    sClass = 'text-success';
                } else {
                    status = 'Pending';
                    sClass = 'text-danger';
                }
                html = '<tr data-req="' + row.id + '"><td>' + row.name + '</td><td>' + row.manager + '</td><td>' + row.date + '</td><td><a href="javascript:void(0)" class="'+sClass+'" data-req="' + row.id + '" class="stat">' + status + '</a></td><td align="center">' + detailsBtn + '</td></tr>';
                $('#ItemShow tbody').append(html);
            });
        }
    });
};

function SaveRequest() {
    var text = $('#detialText').val();
    $.ajax({
        url: '/Item/SendProductRequest',
        type: 'POST',
        data: { requestText: text },
        dataType: 'json',
        success: function (res) {
            alert("success");
            $('#ReqForm').trigger('reset');
        }
    });
};

function showProductReqDetails(reqId) {
    $('.loading').show();
    $.ajax({
        url: '/Item/GetProductRequestDetails',
        type: 'POST',
        dataType: 'json',
        data: { requestId: parseInt(reqId) },
        success: function (res) {
            $('.detais-box').show();
            $('#reqDetails').empty();
            setTimeout(function () {
                $('.loading').hide();
                $('.btn-approved').show();
                var html;
                html = '<pre data-req="' + res[0].id + '">' + res[0].details + '</pre>';
                $('#reqDetails').append(html);
            }, 1000);
        }
    });
};

function ApprovedRequestByAdmin(id) {
    $.ajax({
        url: '/Item/ApprovedProRequestByAdmin',
        type: 'POST',
        data: { rid: id },
        dataType: 'json',
        success: function (res) {
            if (res === true) {
                GetProductRequest();
            } else {
                alert("Something Wrong!");
            }
        }
    });
};
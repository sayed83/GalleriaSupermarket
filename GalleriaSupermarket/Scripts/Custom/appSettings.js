$(document).ready(function () {
    var outletId = parseInt(JSON.parse(localStorage.getItem("OutletID")));

    hidePopupIsOutletAlreadySet();

    $.ajax({
        url: '/Home/GetCity',
        type: 'GET',
        dataType: 'json',
        success: function (res) {
            var html;
            $.each(res, function (index, row) {
                html = '<option value="' + row.id + '">' + row.name + '</option>';
                $('#OutletCity').append(html);
            })
        }
    });

    $('#OutletCity').change(function () {
        var id = parseInt($(this).val());
        $.ajax({
            url: '/Home/GetOutletLocation',
            type: 'POST',
            data:{cid:id},
            dataType: 'json',
            success: function (res) {
                $('#OutletLoc').empty();
                var html;
                var common = { id: 0, location: '--- Select Location ---' }
                res.splice(0, 0, common);
                $.each(res, function (index, row) {
                    html = '<option value="' + row.id + '">' + row.location + '</option>';
                    $('#OutletLoc').append(html);
                })
            }
        });
    });

    $('#cngLoc').click(function () {
        $('#popupBox').show();
    });

    $('#popupClose').click(function () {
        $('#popupBox').hide();
    });

    $('#cod-option').on('click', function () {
        $('#cod-img, #on-payment, #pmnt-more').hide();
        $('#cod-payment, #on-img').fadeIn(1500);
    });

    $('#onpmnt-option').on('click', function () {
        $('#on-img, #on-payment,#cod-payment').hide();
        $('#pmnt-more, #cod-img').fadeIn(1500);
    });
    $('#more-bks').on('click', function () {
        $('#pmnt-more').hide();
        $('#on-payment').fadeIn(1500);
    });

    function hidePopupIsOutletAlreadySet() {
        if (outletId == 0) {
            $('#popupBox').show();
            $('#FruitsSection').show();
        } else if (outletId > 0) {
            $('#popupBox').hide();
            $('#FruitsSection').hide();
        } else {
            $('#OutletSection').hide();
            localStorage.setItem("OutletID", 0);
        }
    };

});                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
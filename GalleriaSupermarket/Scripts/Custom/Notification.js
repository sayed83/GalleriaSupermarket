$(function () {
    // Click on notification icon for show notification  
    $('.noti').click(function () {
        updateNotification();
    });

    $('.req-noti').click(function () {
        GetProReqNotifications();
    });
    GetProReqNotifications();
    updateNotification();
    setInterval(function () {
        countNoti();
        reqCountNoti();
        CountTransferRequest();
    }, 1000);
    // update notification 
    
    function CountTransferRequest() {
        $.ajax({
            url: '/ItemTransfer/GetItemTransferRequest',
            type: 'GET',
            dataType:'json',
            success: function (res) {
                $('.countRequest').empty();
                if (res.length != 0 && res != false) {
                    $('.countRequest').show();
                    $('.countRequest').text(res);
                }
            }
        });
    };

    function updateNotification() {
        $('#notiContent').empty();
        $('#notiContent').append($('<li>Loading...</li>'));

        $.ajax({
            type: 'GET',
            url: '/Home/GetNotifications',
            success: function (response) {
                $('#notiContent').empty();
                if (response.length == 0) {
                    $('#notiContent').append($('<li>Currently You Have No New Notifications.</li>'));
                }
                $.each(response, function (index, value) {
                    var html;
                    html = '<li data-id="' + value.id + '"><a href="#"><i class="fa fa-warning text-yellow"></i> ' + value.name + ' <span style="color:red">Qnty</span> ' + value.qnty + '</a></li>';
                    $('#notiContent').append(html);
                });
            },
            error: function (error) {
                console.log(error);
            }
        });
    };
    // update notification count
    countNoti();
    function countNoti() {
        $.ajax({
            url: '/Home/NotficationCount',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                $('.not-count').text(res);
                $('.header-count').text(res);
                $('#header-count').text('You have ' +res+ ' notifications');
                if ($('.not-count').text() == 0) {
                    $('.not-count').empty()
                }
            }
        });
    };

    reqCountNoti();
    function reqCountNoti() {
        $.ajax({
            url: '/Home/ReqNotficationCount',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                $('.req-count').text(res);
                $('#reqheader-count').text('You have ' + res + ' notifications');
                if ($('.req-count').text() == 0) {
                    $('.req-count').empty()
                }

            }
        });
    };

    function GetProReqNotifications() {
        $('#notiReq').empty();
        $('#notiReq').append($('<li>Loading...</li>'));

        $.ajax({
            type: 'GET',
            url: '/Home/GetProductReqNotifications',
            success: function (response) {
                $('#notiReq').empty();
                if (response.length == 0) {
                    $('#notiReq').append($('<li>Currently You Have No New Notifications.</li>'));
                }
                $.each(response, function (index, value) {
                    var html;
                    html = '<li data-id="' + value.id + '"><a href="/Item/ProductRequest"><i class="fa fa-shopping-cart text-green"></i> Product Request From <strong style="color:#00a659;">' + value.name + '</strong></a></li>';
                    $('#notiReq').append(html);
                });
            },
            error: function (error) {
                console.log(error);
            }
        })
    }

});
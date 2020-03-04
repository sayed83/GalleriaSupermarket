    $(document).ready(function () {
        homeNamespace.init();
    });

    (function () {
        this.homeNamespace = this.homeNamespace || {};
        var ns = this.homeNamespace;

        ns.init = function () {
            var home = new ns.Homepage();

            $('.mnodeList').on('click', '.CatID', function () {
                $(this).siblings('.mtree-level-1').slideToggle();
                $(this).children('.minusicon, .plusicon').toggle();
            });
            $('.mnodeList').on('click', '.node1', function () {
                $(this).siblings('.mtree-level-2').slideToggle();
                $(this).children('.node1minusicon, .node1plusicon').toggle();
            });
            $('.mnodeList').on('click', '.brand-id', function () {
                var id = $(this).attr('data-id');
                home.GetItemsbyBrandID(id);
            });

            //home.GetBrandBySubCate();node1
            $('#OutletLoc').change(function () {
                var id = parseInt($(this).val());
                home.GetItemsbyOutlet('.outlet-slide', id);
            });
            
            $(document).on('change', '.SizeList', home.SizeWisePriceCalculation);

            if (JSON.parse(localStorage.getItem("OutletID")) > 0) {
                home.GetItemsbyOutlet('.outlet-slide', JSON.parse(localStorage.getItem("OutletID")));
                $('#FruitsSection').hide();
                $('#OutletSection').show();
            } else {
                home.GetItemsByCategory('.cat-slide');
                $('#OutletSection').hide();
            }
            home.Getcategories();

            $('#Reset').click(function () {
                localStorage.setItem("OutletID", 1);
                home.GetItemsByCategory('.cat-slide');
                $('#OutletSection').hide();
                $('#FruitsSection').show();
                $('#popupBox').hide();
            });
                        
        };

        ns.Homepage = (function () {

            function Homepage() { }//constructor

            Homepage.prototype.GetItemsbyOutlet = function (appendDiv, pid) {
                $('#loading').show();
                setTimeout(function () {   //calls click event after a certain time
                    $.ajax({
                        url: '/Home/GetItemsByOutlet',
                        type: 'POST',
                        data: { id: pid },
                        cache: false,
                        dataType: 'json',
                        success: function (res) {
                            $('' + appendDiv + '').empty();
                            var div;
                            if (res.length > 0) {
                                $('#popupBox').hide();
                                localStorage.setItem("OutletID", JSON.stringify(pid));
                                $.each(res, function (index, row) {
                                    div = '<div class="col-md-4 product-men"><div class="men-pro-item simpleCart_shelfItem"><div class="men-thumb-item"><img class="img-responsive" src="/App_File/ProductImage/' + row.image + '" /></div><div class="item-info-product "><h4><a href="#">' + row.name + '</a></h4><div class="info-product-price"><span class="item_price">$' + row.price + '</span></div><div data-actualPrice="" class="sizebox">Size:: <select data-price="'+row.price+'" data-proid="' + row.proid + '" id="ProId' + row.proid + '" class="SizeList"><option value="0">Size</option></select></div><a class="btn btn-info addtocart" data-imgUrl="' + row.image + '" data-imageid="' + row.imageId + '" data-outlet="' + row.outletid + '" data-id="' + row.id + '" data-item="' + row.name + '" data-price="' + row.price + '" data-count="1" href="javascript:void(0)">Add to Cart</a></div></div></div>';

                                    $('' + appendDiv + '').append(div);

                                });
                               
                                $('.SizeList').each(function () {
                                    var $this = $(this);
                                    var id = $(this).attr('data-proid');
                                    id = parseInt(id);
                                    $.ajax({
                                        url: '/Home/GetSize',
                                        type: 'POST',
                                        datatype: 'json',
                                        data: { prosid: id },
                                        success: function (data) {
                                            $($this).empty();
                                            var output;
                                            var common = { id: 0, size: 'size' }
                                            data.splice(0, 0, common);
                                            $.each(data, function (i, r) {
                                                output = '<option value="' + r.size + '">' + r.size + '</option>';
                                                $($this).append(output);
                                            });
                                        }
                                    });
                                        
                                });

                                var divs = $('' + appendDiv + ' > div');
                                for (var i = 0; i < divs.length; i += 8) {
                                    divs.slice(i, i + 8).wrapAll("<div class='new'></div>");
                                }
                            } else {
                                $('' + appendDiv + '').parent().hide();
                            }

                            $('' + appendDiv + '').owlCarousel({
                                loop: true,
                                margin: 0,
                                dots: false,
                                nav: true,
                                responsiveClass: true,
                                lazyLoad: true,
                                responsive: {
                                    0: { items: 1, nav: true },
                                    600: { items: 3, nav: false },
                                    1000: { items: 1, nav: true, loop: false }
                                }
                            });
                            $('#FruitsSection, #FashionSection').hide();
                            $('#OutletSection').show();
                        },
                        complete: function () {
                            $('#loading').hide();
                        }
                    });
                }, 2000);
                
            };

            Homepage.prototype.SizeWisePriceCalculation = function () {
                var $this = $(this);
                var oldVal = $this.val();
                var size = $this.val();
                size = parseFloat(size.replace('/g|kg|L|ml/gi', ''));//gi means req exp pattern caseinsensetive

                var price = $this.attr('data-price');
                price = parseFloat(price);

                if (size === 250) {
                    $this.attr('data-actualprice', Number(price / 4));
                    $this.parent().siblings('.info-product-price').find('.item_price').text('$' + Number(price / 4));
                } else if (size === 500 || size===25) {
                    $this.attr('data-actualprice', Number(price / 2));
                    $this.parent().siblings('.info-product-price').find('.item_price').text('$' + Number(price / 2));
                } else if (size === 2) {
                    $this.attr('data-actualprice', Number(price * 2));
                    $this.parent().siblings('.info-product-price').find('.item_price').text('$' + Number(price * 2));
                } else if (size === 5) {
                    $this.attr('data-actualprice', Number(price * 5));
                    $this.parent().siblings('.info-product-price').find('.item_price').text('$' + Number(price * 5));
                } else {
                    $this.attr('data-actualprice', Number(price * 1));
                    $this.parent().siblings('.info-product-price').find('.item_price').text('$' + Number(price * 1));
                }
            };

            function Getsubcategory(appendDiv, id) {
                $.ajax({
                    url: '/Home/GetSubCategories',
                    type: 'POST',
                    dataType: 'json',
                    data: { id: parseInt(id) },
                    success: function (res) {
                        var html;
                        $(appendDiv).empty();
                        $.each(res, function (index, row) {
                            html = '<li class="mtree-node"><a data-id="' + row.id + '" class="node1" href="javascript:void(0)">' + row.name + '<i class="fa fa-angle-right icontog node1plusicon" aria-hidden="true"></i><i style="display:none;" class="fa fa-angle-down icontog node1minusicon" aria-hidden="true"></i></a><ul style="display:none;" class="mtree-level-2"></ul></li>';
                            $(appendDiv).append(html);
                        });
                    }
                });
            };

            function GetBrandBySubCate(appendDiv, id) {
                $.ajax({
                    url: '/Home/GetBrandBySubcategory',
                    type: 'POST',
                    data: { subid: parseInt(id) },
                    dataType: 'json',
                    success: function (res) {
                        var html;
                        $(appendDiv).empty();
                        $.each(res, function (index, row) {
                            html = '<li><a class="brand-id" data-id="' + row.id + '" href="javascript:void(0)">' + row.name + '</a></li>';
                            $(appendDiv).append(html);
                        });
                    }
                });
            };

            Homepage.prototype.GetItemsbyBrandID = function (brandid) {
                $.ajax({
                    url: '/Home/GetItems',
                    type: 'POST',
                    data: { bid: parseInt(brandid) },
                    dataType: 'json',
                    success: function (res) {
                        var resData = $.isEmptyObject(res);//check array empty or not :: return true or false
                        $('#SearchProduct, .loadingImg').show();
                        //$('#BrandProduct').hide();

                        setTimeout(function () {
                            $('.SearchItems').empty();
                            var html;
                            $('#OutletSection, .loadingImg').hide();
                            if (resData === false) {
                                $('#BrandProduct').show();
                                $('#BrandProduct').text('Search For ' + res[0].brand + ' Products');
                                $.each(res, function (index, row) {
                                    html = '<div class="col-md-4 product-men"><div class="men-pro-item simpleCart_shelfItem"><div class="men-thumb-item"><img class="img-responsive" src="/App_File/ProductImage/' + row.image + '" /></div><div class="item-info-product "><h4><a href="#">' + row.name + '</a></h4><div class="info-product-price"><span class="item_price">$' + row.price + '</span><del>$' + row.price + '</del></div><div data-actualPrice="" class="sizebox">Size:: <select data-proid="' + row.id + '" class="SizeList"><option value="0">Size</option></select></div><a class="btn btn-info addtocart" data-imgUrl="' + row.image + '" data-imageid="' + row.imageId + '" data-outlet="' + row.outletid + '" data-id="' + row.id + '" data-item="' + row.name + '" data-price="' + row.price + '" data-count="1" href="javascript:void(0)">Add to Cart</a></div></div></div>';
                                    $('.SearchItems').append(html);
                                });

                                $('#SearchProduct').find('.SizeList').each(function () {
                                    var $this = $(this);
                                    var id = $(this).attr('data-proid');
                                    id = parseInt(id);
                                    $.ajax({
                                        url: '/Home/GetSize',
                                        type: 'POST',
                                        datatype: 'json',
                                        data: { prosid: id },
                                        success: function (data) {
                                            $($this).empty();
                                            var output;
                                            $.each(data, function (i, r) {
                                                output = '<option value="' + r.size + '">' + r.size + '</option>';
                                                $($this).append(output);
                                            });
                                        }
                                    });
                                });


                            } else {

                                $('#BrandProduct').hide();
                                html = '<h3>No Product Available for this Category</h3>';
                                $('.SearchItems').append(html);
                            }
                        }, 1000);
                    }
                });
            };

            Homepage.prototype.Getcategories = function () {
                $.ajax({
                    url: '/Home/GetCategories',
                    type: 'GET',
                    dataType: 'json',
                    success: function (res) {
                        $('#accordion').empty();
                        var output;
                        $.each(res, function (index, row) {
                            output = '<li class="mtree"><a data-id="' + row.id + '" id="CatID'+row.id+'" class="CatID" href="javascript:void(0)"><span class="cat-text">' + row.name + '</span><i class="fa fa-plus icontog plusicon" aria-hidden="true"></i><i style="display:none;" class="fa fa-minus icontog minusicon" aria-hidden="true"></i></a><ul style="display:none;" class="mtree-level-1"></ul></li>';
                            $('.mnodeList').append(output);
                        });
                        
                        $('.CatID').each(function () {
                            var $this = $(this);
                            var div = $this.siblings('.mtree-level-1');
                            var id = parseInt($this.attr('data-id'));
                            Getsubcategory(div, id);
                        });

                        setTimeout(function () {
                            $('.node1').each(function () {
                                var $this = $(this);
                                var sDiv = $this.siblings('.mtree-level-2');
                                var subId = parseInt($this.attr('data-id'));
                                GetBrandBySubCate(sDiv, subId);
                            });
                        }, 1000)
                        
                    }
                });
            };

            Homepage.prototype.GetItemsByCategory = function (appendDiv) {
                $.ajax({
                    url: '/Home/GetItemsByCategory',
                    type: 'GET',
                    dataType: 'json',
                    success: function (res) {
                        $('' + appendDiv + '').empty();
                        var div;
                        if (res.length > 0) {
                            //localStorage.setItem("OutletID", 1);
                            $.each(res, function (index, row) {
                                div = '<div class="col-md-4 product-men"><div class="men-pro-item simpleCart_shelfItem"><div class="men-thumb-item"><img class="img-responsive" src="/App_File/ProductImage/' + row.image + '" /></div><div class="item-info-product "><h4><a href="#">' + row.name + '</a></h4><div class="info-product-price"><span class="item_price">$' + row.price + '</span></div><div data-actualPrice="" class="sizebox">Size:: <select data-price="' + row.price + '" data-proid="' + row.proid + '" id="ProId' + row.proid + '" class="SizeList"><option value="0">Size</option></select></div><a class="btn btn-info addtocart" data-imgUrl="' + row.image + '" data-imageid="' + row.imageId + '" data-outlet="' + row.outletid + '" data-id="' + row.id + '" data-item="' + row.name + '" data-price="' + row.price + '" data-count="1" href="javascript:void(0)">Add to Cart</a></div></div></div>';

                                $('' + appendDiv + '').append(div);

                            });

                            $('.SizeList').each(function () {
                                var $this = $(this);
                                var id = $(this).attr('data-proid');
                                id = parseInt(id);
                                $.ajax({
                                    url: '/Home/GetSize',
                                    type: 'POST',
                                    datatype: 'json',
                                    data: { prosid: id },
                                    success: function (data) {
                                        $($this).empty();
                                        var output;
                                        $.each(data, function (i, r) {
                                            output = '<option value="' + r.size + '">' + r.size + '</option>';
                                            $($this).append(output);
                                        });
                                    }
                                });

                            });

                            var divs = $('' + appendDiv + ' > div');
                            for (var i = 0; i < divs.length; i += 4) {
                                divs.slice(i, i + 4).wrapAll("<div class='new'></div>");
                            }


                        } else {
                            $(''+appendDiv+'').parent().hide();
                        }
                        
                        $('' + appendDiv + '').owlCarousel({
                            loop: true,
                            items: 1,
                            margin: 0,
                            dots: false,
                            nav: true,
                            responsiveClass: true,
                            lazyLoad: true,
                            responsive: {
                                0: { items: 1, nav: true },
                                600: { items: 1, nav: false },
                                1000: { items: 1, nav: true, loop: false }
                            }
                        });
                    }
                });
            };

            return Homepage;
        }());

    })(); 
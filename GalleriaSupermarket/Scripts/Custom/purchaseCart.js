/// <reference path="../jquery-3.3.1.min.js" />

$(document).ready(function () {
    shoppingCart.init();

});

(function () {
    this.shoppingCart = this.shoppingCart || {};
    var ns = this.shoppingCart;

    ns.init = function () {
        var cart = new ns.Cart();
        cart.getSupplier();
        cart.getCategories();
        cart.loadCart();
        cart.purchaseCalculateDue();
        $('#CategoryID').on('change', function () {
            var id = $(this).val();
            cart.getSubCategoriesforPurchase(id);
        });
        $("#invTable").on("change", ".pcngPrice", function () {
            var name = $(this).attr("data-name");
            var id = parseInt($(this).parents('tr').attr('data-id'));
            var newQnty = parseInt($(this).val());
            cart.calculatePriceFromInput(id, newQnty);
            cart.purchaseCalculateDue();
            cart.calculateNetAmount();
        });

        $('.payment-details').on('change', '#Paidamnt', cart.purchaseCalculateDue);

        $("#invTable").on("change", ".pinQnty", function () {
            var name = $(this).attr("data-name");
            var id = parseInt($(this).parents('tr').attr('data-id'));
            var newPrice = parseFloat($(this).val());
            cart.calculateQntyFromInput(id, newPrice);
            cart.purchaseCalculateDue();
            cart.calculateNetAmount();
        });
        $('#shippingCost, #Paidamnt').change(function () {
            cart.calculateNetAmount();
        });
        $(document).on('click', '.addcart', cart.bindItemToCart);
        $('#Submit').on('click', cart.submitOrder);
        $('#showSuccess').hide();
        $('#showError').hide();
        $(document).on('click', '.deleteBtn', cart.deleteItemFromBox);
        $(document).on('click', '.subBtn', cart.cartItemOrderRemove);
        $(document).on('click', '.addBtn', cart.cartItemOrder);
    };

    ns.Cart = (function () {
        function Cart() { };//constructor
        var cart = [];

        function Item(id, name, price, count) {
            this.id = id;
            this.name = name;
            this.price = price;
            this.count = count;
        };

        // Save cart to the local Storage
        function saveCart() {
            localStorage.setItem("purchaseCart", JSON.stringify(cart));
        };
        //Load cart Funciton
        function loadCart() {
            cart = JSON.parse(localStorage.getItem("purchaseCart"));
        };

        loadCart();

        function addItemToCart(id, name, price, count) {
            for (var i in cart) {
                if (cart[i].id === id) {
                    cart[i].count += count;
                    saveCart();
                    displayCart();
                    totalCartPrice();
                    return;
                }
            }
            var item = new Item(id, name, price, count);
            if (cart == null) {
                cart = [];
                cart.push(item);
            } else {
                cart.push(item);
            }
            saveCart();
            displayCart();
            totalCartPrice();
        };

        function removeItemFromCart(id) {
            for (var i in cart) {
                if (cart[i].id === parseInt(id)) {
                    cart[i].count--;
                    if (cart[i].count < 1) {
                        cart.splice(i, 1);
                    }
                    break;
                }
            }
            saveCart();
        };

        //Remove All Item form Cart
        function removeItemFromCartAll(id) {
            for (var i in cart) {
                if (cart[i].id === parseInt(id)) {
                    cart.splice(i, 1);
                    break;
                }
            }
            saveCart();
        };

        //List Item for ui binding
        function listCart() {
            var cartCopy = [];
            for (var i in cart) {
                var item = cart[i];
                var itemCopy = {};
                for (var p in item) {
                    itemCopy[p] = item[p];
                }
                cartCopy.push(itemCopy);
            }
            return cartCopy;
        };

        //Clear cart
        function clearCart() {
            cart = [];
            saveCart();
        };

        //count total Item
        function totalCountCart() {
            var totalCount = 0;
            for (var i in cart) {
                totalCount += cart[i].count;
            }
            return totalCount;
        };

        //single product totalPrice
        function perItemTotalPrice(id) {
            var totalCost = 0;
            for (var i in cart) {
                if (cart[i].id === id) {
                    totalCost = cart[i].price * cart[i].count;
                }
            }
            return totalCost.toFixed(2);
        };
        //total cart price
        function totalCartPrice() {
            var totalCost = 0;
            for (var i in cart) {
                var itemPrice = cart[i].price * cart[i].count;
                totalCost += itemPrice;
            }
            return totalCost.toFixed(2);
        };

        Cart.prototype.loadCart = function () {
            displayCart();
        };

        function displayCart() {
            $('#GrossAmount').html('$' + totalCartPrice());
            var cartArray = listCart();
            var output = "";
            for (var i in cartArray) {
                output += "<tr class='item-row' id='" + cartArray[i].id + "' data-id='" + cartArray[i].id + "' data-price='" + cartArray[i].price + "' data-count='" + cartArray[i].count + "' data-name='" + cartArray[i].name + "'><td><button type='button' data-itemid='" + cartArray[i].id + "' class='btn btn-primary deleteBtn'><i class='fa fa-trash-o'></i></button></td><td>" + cartArray[i].name + "</td>";
                output += "<td><input type='text' data-price='" + cartArray[i].price + "' data-name='" + cartArray[i].name + "' class='form-control pcngPrice' value='" + cartArray[i].price + "'/></td><td><button type='button' class='btn btn-info calBtn subBtn' data-item='" + cartArray[i].id + "'>-</button></td><td id='InvQty" + cartArray[i].id + "'><div class='calbtn-box'><span class='proQty'><input class='pinQnty form-control' data-count='" + cartArray[i].count + "' data-id='"+cartArray[i].id+"' data-name='" + cartArray[i].name + "' type='text' value='" + cartArray[i].count + "'/></span></div></td><td><button type='button' class='btn btn-info calBtn addBtn' data-item='" + cartArray[i].id + "'>+</button></td><td class='TotalCost'>" + perItemTotalPrice(cartArray[i].id) + "</td></tr>";
            }
            $('#invTable tbody').html(output);
        };

        Cart.prototype.calculatePriceFromInput = function (id, newprice) {
            for (var i in cart) {
                if (cart[i].id == id) {
                    cart[i].price = newprice;
                    break;
                }
            }
            saveCart();
            displayCart();
        };

        Cart.prototype.removeCartItemOnchange = function (id) {
            removeItemFromCart(id)
        }

        Cart.prototype.calculateQntyFromInput = function (id, newQnty) {
            for (var i in cart) {
                if (cart[i].id == id) {
                    cart[i].count = newQnty;               
                    break;
                }
            }
            saveCart();
            displayCart();
        }

        Cart.prototype.bindItemToCart = function () {
            var itemid = parseInt($(this).attr("data-id"));
            var itemname = $(this).attr("data-name");
            var itemprice = parseFloat($(this).attr("data-price"));
            var itemcount = parseInt($(this).attr("data-count"));
            addItemToCart(itemid, itemname, itemprice, itemcount);
        };


        Cart.prototype.submitOrder = function () {
            var Products = [];
            var supplier = parseInt($('#SupplierID').val());
            var outlet = parseInt($('#OutletID').val());
            var paid = parseFloat($('#Paidamnt').val());
            var dueamnt = $('.dueAmnt').text();
            dueamnt = parseFloat(dueamnt.replace('$', '0'));
            var totalamnt = $('#GrossAmount').text();
            totalamnt = parseFloat(totalamnt.replace('$', '0'));
            var shipCost = parseFloat($('#shippingCost').val());
            var netamnt = $('#NetAmount').text();
            netamnt = parseFloat(netamnt.replace('$', '0'));

            $("tr.item-row").each(function () {
                $this = $(this);
                var itemid = parseInt($this.attr("data-id"));
                var price = parseFloat($this.attr("data-price"));
                var qnty = parseInt($this.attr("data-count"));
                Products.push({ SubCategoryID: itemid, Price: price, Quantity: qnty });
            });

            if (outlet > 0 || supplier > 0) {
                $.ajax({
                    url: '/Purchase/Create',
                    type: 'POST',
                    traditional: true,
                    data: { purOrder: JSON.stringify(Products), shipCost: shipCost, supplierId: supplier, outletId: outlet, totalAmnt: totalamnt, paidamnt: paid, netAmount: netamnt, dueamnt: dueamnt },
                    dataType: 'json',
                    success: function (res) {
                        $("#OutletID").val("");
                        $("#SupplierID").val("");
                        $('#showSuccess').show();
                        clearCart();
                        displayCart();
                    }
                });
            } else {
                $('#showError').show();
            }
        };

        Cart.prototype.purchaseCalculateDue = function () {
            CalculateDue();
            CalculateNetTotal();
        };
        function CalculateDue() {
            var grossamnt = $('#GrossAmount').text();
            grossamnt = parseFloat(grossamnt.replace('$', '0'));
            var paid = parseFloat($('#Paidamnt').val());

            $('.dueAmnt').text('$' + Number(grossamnt - paid));
        };

        Cart.prototype.calculateNetAmount = function () {
            CalculateNetTotal();
        };

        function CalculateNetTotal() {
            var shipping = parseFloat($('#shippingCost').val());
            var dueamnt = $('.dueAmnt').text();
            dueamnt = parseFloat(dueamnt.replace('$', '0'));

            $('#NetAmount').text('$' + Number(shipping + dueamnt));
        };

        Cart.prototype.cartItemOrder = function () {
            var pid = parseInt($(this).attr("data-item"));
            addItemToCart(pid, '', 0, parseInt(1));
            CalculateDue();
            CalculateNetTotal();
            displayCart();
        };

        Cart.prototype.cartItemOrderRemove = function () {
            var id = $(this).attr("data-item");
            removeItemFromCart(id);
            CalculateDue();
            CalculateNetTotal();
            displayCart();
        };

        Cart.prototype.deleteItemFromBox = function () {
            var id = $(this).attr("data-itemid");
            removeItemFromCartAll(id);
            displayCart();
            CalculateDue();
            CalculateNetTotal();
        };

        //cart for purchase invoice
        Cart.prototype.getSupplier = function () {
            $.ajax({
                url: '/Purchase/GetSupplier',
                type: 'GET',
                dataType: 'json',
                success: function (res) {
                    var html;
                    $.each(res, function (index, row) {
                        html = '<option value="' + row.id + '">' + row.name + '</option>';
                        $('#SupplierID').append(html);
                    });

                }
            })
        };

        Cart.prototype.getCategories = function () {
            $.ajax({
                url: '/Home/GetCategories',
                type: 'GET',
                dataType: 'json',
                success: function (res) {
                    $('#CategoryID').empty();
                    var html;
                    var resData = $.isEmptyObject(res);//check array empty or not :: return true or false
                    if (resData === true) {
                        html = '<option value="0">No Data Available</option>';
                        $('#CategoryID').append(html);
                    } else {
                        var common = { id: 0, name: '---- Select Category ----' };
                        //res.splice(position, numberOfItemToRemove, item)
                        res.splice(0, 0, common); //Add array object
                        $.each(res, function (index, row) {
                            html = '<option value="' + row.id + '">' + row.name + '</option>';
                            $('#CategoryID').append(html);
                        })
                    }

                }
            })
        };

        Cart.prototype.getSubCategoriesforPurchase = function (cid) {
            $('#PurchaseProduct tbody').empty();
            $.ajax({
                url: '/Home/GetSubCategories',
                type: 'POST',
                data: { id: cid },
                dataType: 'json',
                success: function (res) {
                    var html;
                    var resData = $.isEmptyObject(res);//check array empty or not :: return true or false
                    if (resData === true) {
                        html = '<tr><td colspan="4">No Product Available</td></tr>';
                        $('#PurchaseProduct tbody').append(html);
                    } else {
                        $.each(res, function (i, row) {
                            html = '<tr>';
                            html += '<td class="proName">' + row.name + '</td>';
                            html += '<td><button type="button" id="Btn' + row.id + '" data-id="' + row.id + '" data-name="' + row.name + '" data-count="1" data-price="0" class="btn btn-primary addcart">+</button></td>';
                            html += '</tr>';
                            $('#PurchaseProduct tbody').append(html);
                        });
                    }

                }
            });
        }

        Cart.prototype.saleItemsDatatable = function () {
            $('#SaleProduct, #PurchaseProduct').DataTable({
                'aoColumnDefs': [{
                    'bSortable':false,
                    'aTargets':['nosort']
                }]
            });
        };


        return Cart;
    }());


})();


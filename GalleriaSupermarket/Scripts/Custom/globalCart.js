/// <reference path="../jquery-3.3.1.min.js" />
$(document).ready(function () {
    globalCart.init();
    
});

(function () {
    this.globalCart = this.globalCart || {};
    var ns = this.globalCart;

    ns.init = function () {
        var gbCart = new ns.Cart();
        gbCart.bindCarttoModal();
        gbCart.countCartItem();
        gbCart.displayCartInCheckout();
        gbCart.displayCartInPaymentpage();
        $('#ckPmnt').on('click', gbCart.checkUserLoginOrNot)
        $('#viewCart').on('click', gbCart.showUserCart);
        $('#cartHide').on('click', gbCart.hideUserCart);
        $('#paymentSubmit').on('click', gbCart.submitOrder);
        $(document).on('click', '.subBtn', gbCart.cartItemOrderRemove);
        $(document).on('click', '.addBtn', gbCart.cartItemOrder);
        $(document).on('click', '.deleteBtn', gbCart.deleteItemFromBox);
        $(document).on('click', '.addtocart', gbCart.bindItemToCart);
    };

    ns.Cart = (function () {

        function Cart() { };//Constructor
        var cart = [];
        
        function Item(id, image, outletid,size, imgid, name, price, count) {
            this.id = id;
            this.image = image;
            this.outletid = outletid;
            this.size = size;
            this.imgid = imgid;
            this.name = name;
            this.price = price;
            this.count = count;
        };
        
        Cart.prototype.showUserCart = function () {
            $('#cartModal').modal('show');
        };

        Cart.prototype.hideUserCart = function () {
            $('#cartModal').modal('hide');
        };

        loadCart();

        function addItemToCart(id, image, outletid, size, imgid, name, price, count) {
            for (var i in cart) {
                if (cart[i].id === id && cart[i].size == size) {
                    cart[i].count += count;
                    saveCart();
                    totalCartPrice();
                    return;
                }
            }
            var item = new Item(id, image, outletid, size, imgid, name, price, count);
            if (cart == null) {
                cart = [];
                cart.push(item);
            } else {
                cart.push(item);
            }
            saveCart();
            totalCartPrice();
        };
        function removeItemFromCart(id, size) {
            for (var i in cart) {
                if (cart[i].id === parseInt(id) && cart[i].size == size) {
                    cart[i].count--;
                    if (cart[i].count === 0) {
                        cart.splice(i, 1);
                    }
                    break;
                }
            }
            saveCart();
        };
        //Remove All Item form Cart
        function removeItemFromCartAll(id, size) {
            for (var i in cart) {
                if (cart[i].id === parseInt(id) && cart[i].size == size) {
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
        // Save cart to the local Storage
        function saveCart() {                                     
            localStorage.setItem("userCart", JSON.stringify(cart));
        };
        //Load cart Funciton
        function loadCart() {
            cart = JSON.parse(localStorage.getItem("userCart"));
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
            return Math.ceil(totalCost);
        };
        //total cart price
        function totalCartPrice() {
            var totalCost = 0;
            for (var i in cart) {
                var itemPrice = cart[i].price * cart[i].count;
                totalCost += itemPrice;
            }
            return Math.ceil(totalCost);
        };
        
        Cart.prototype.countCartItem = function () {
            if (cart == null) {
                $('.cart-text').html('My Cart');
                $('#ckProduct').html("0 Item");
            } else {
                $('.cart-text').html('My Cart <strong>(' + cart.length + ')</strong>');
                $('#ckProduct').html(cart.length + " Items");
            }
        };

        Cart.prototype.bindItemToCart = function () {
            if ($(this).siblings('.sizebox').find('.SizeList').val() == 'size') {
                $(this).siblings('.sizebox').find('.SizeList').css('border','1px solid #ff5722');
            } else {
                var itemid = parseInt($(this).attr("data-id"));
                var itemname = $(this).attr("data-item");
                var image = $(this).attr("data-imgurl");
                var size = $(this).parents('.product-men').find('.SizeList').val();
                var imageId = parseInt($(this).attr("data-imageid"));
                var outletId = parseInt($(this).attr("data-outlet"));
                var itemprice = $(this).siblings('.info-product-price').find('.item_price').text();
                itemprice = parseFloat(itemprice.replace('$', ''));
                var itemcount = parseInt($(this).attr("data-count"));
                //new Item(id, image, outletid, size, imgid, name, price, count);
                addItemToCart(itemid, image, outletId, size, imageId, itemname, itemprice, itemcount);
                displayCart();
                $('#cartModal').modal('show');
            }            
        };

        Cart.prototype.cartItemOrder = function () {
            var id = parseInt($(this).attr("data-item"));
            var size = $(this).attr('data-size');
            addItemToCart(id, '',0, size, 0, '', 0, 1);
            displayCheckout();
            displayCart();
        };

        Cart.prototype.cartItemOrderRemove = function () {
            var id = parseInt($(this).attr("data-item"));
            var size = $(this).attr('data-size');
            removeItemFromCart(id, size);
            displayCheckout();
            displayCart();
        };

        Cart.prototype.deleteItemFromBox = function () {
            var id = $(this).attr("data-itemid");
            var size = $(this).attr('data-size');
            removeItemFromCartAll(id, size);
            displayCheckout();
            displayCart();
        };

        Cart.prototype.checkUserLoginOrNot = function () {
            $.ajax({
                url: '/Home/UserAuthenticate',
                type: 'GET',
                dataType: 'json',
                success: function (res) {
                    if (res === false) {
                        window.location.href = '/Account/Login?ReturnUrl=%2FHome%2FCheckOut';
                    }
                }
            });
        }

        Cart.prototype.submitOrder = function () {
            var Products = [];
            var customerName = $('#fullName').val();
            var custPhone = $('#contactNumber').val();
            var city = $('#getCity').val();
            var addr = $('#getAddr').val();
            var payerNo = $('.bNumber').val();
            var tranNo = $('.tranNo').val();
            var payment = $('#paymentSubmit').attr("data-pmnt");
            var outlet = JSON.parse(localStorage.getItem("OutletID"));
            var pMethod = $('#paymentSubmit').attr("data-method");

            $("tr.order-row").each(function () {
                $this = $(this);
                var itemid = parseInt($this.attr("data-id"));
                var qnty = parseInt($this.attr("data-count"));
                var size = $this.attr('data-size');
                var price = parseFloat($this.attr('data-price'));
                Products.push({ ItemID: itemid, Quantity: qnty, ItemSize: size, ItemPrice: price });
            });
            if (payerNo == null || payerNo =='' && tranNo ==null || tranNo =='') {
                alert("Something Wrong!");
            } else {
                $.ajax({
                    url: '/OnlineSale/Create',
                    type: 'POST',
                    traditional: true,
                    data: { order: JSON.stringify(Products), PaymentType: payment, PayerNumber: payerNo, Outlet:outlet, TransNumber: tranNo, PaymentMethod: pMethod, City: city, Address: addr, ContactNumber: custPhone, FullName: customerName },
                    dataType: 'json',
                    success: function (res) {
                        if (res == true) {
                            $('#showSuccess').show();
                            clearCart();
                            displayCart();
                            displayCheckout();
                            sessionStorage.setItem("Address", []);
                            window.location.href = '/Home/Thankyou';
                        } else {
                            $('#tran-alert').show();
                            setTimeout(function () {
                                $('#tran-alert').hide();
                            }, 2000);
                        }
                        
                    }
                });
            }


        };

        function displayCart () {
            $('#CartTotal').html('Sub Total :: $' + totalCartPrice());
            if (cart == null) {
                $('.cart-text').html('My Cart');
            } else {
                $('.cart-text').html('My Cart <strong>(' + cart.length + ')</strong>');
            }
           
            var cartArray = listCart();
            var output ='';
            for (var i in cartArray) {
                output += '<tr><td><button type="button" data-size="' + cartArray[i].size + '" data-itemid="' + cartArray[i].id + '" class="btn btn-primary deleteBtn">X</button></td><td><img width="50" height="43" src="../App_File/ProductImage/' + cartArray[i].image + '"/></td><td>' + cartArray[i].name + '</td><td>' + cartArray[i].size + ' </td><td>$' + cartArray[i].price + '</td><td><button type="button" class="btn btn-info calBtn subBtn" data-size="' + cartArray[i].size + '" data-item="' + cartArray[i].id + '">-</button></td><td>' + cartArray[i].count + '</td><td><button type="button" class="btn btn-info calBtn addBtn" data-item="' + cartArray[i].id + '" data-size="' + cartArray[i].size + '">+</button></td><td>$' + perItemTotalPrice(cartArray[i].id) + '</td></tr>';                
            }
            $('#cartBox tbody').html(output);
            
        };

        function displayCartinPaymentpage() {
            $('#sumTotal').html('$' + totalCartPrice());
            var ca = listCart();
            var output;
            for (var i in ca) {
                output = '<tr class="order-row" data-size="' + ca[i].size + '" data-id="' + ca[i].id + '" data-price="' + perItemTotalPrice(ca[i].id) + '" data-count="' + ca[i].count + '"><td><img width="50" height="43" src="../App_File/ProductImage/' + ca[i].image + '"/></td><td>' + ca[i].name + ' (Size: ' + ca[i].size + ' Qnty: ' + ca[i].count + ' pcs)</td><td>$' + perItemTotalPrice(ca[i].id) + '</td></tr>';
                $('#orderSum tbody').append(output);
            }
        };

        function displayCheckout() {
            $('#grossTotal').html('$' + totalCartPrice());
            if (cart == null) {
                $('#ckProduct').html("0 Product");
                $('.cart-text').html('My Cart');
            } else {
                $('#ckProduct').html(cart.length + " Product");
                $('.cart-text').html('My Cart <strong>(' + cart.length + ')</strong>');
            }
            
            var cartArray = listCart();
            var output = "";
            for (var i in cartArray) {
                output += '<tr><td><button type="button" data-size="' + cartArray[i].size + '" data-itemid="' + cartArray[i].id + '" class="btn btn-primary deleteBtn">X</button></td><td><img width="50" height="43" src="../App_File/ProductImage/' + cartArray[i].image + '"/></td><td>' + cartArray[i].name + '</td><td>' + cartArray[i].size + '</td><td>$' + cartArray[i].price + '</td><td><button type="button" class="btn btn-info calBtn subBtn" data-size="' + cartArray[i].size + '" data-item="' + cartArray[i].id + '">-</button></td><td>' + cartArray[i].count + '</td><td><button type="button" class="btn btn-info calBtn addBtn" data-size="' + cartArray[i].size + '" data-item="' + cartArray[i].id + '">+</button></td><td>$' + perItemTotalPrice(cartArray[i].id) + '</td></tr>';
            }
            $('#checkoutTable tbody').html(output);
        }

        Cart.prototype.displayCartInCheckout = function () {
            displayCheckout();
        };

        Cart.prototype.displayCartInPaymentpage = function () {
            displayCartinPaymentpage();
        };

        Cart.prototype.bindCarttoModal = function () {
            displayCart();
        }

        return Cart;
    }());

})();
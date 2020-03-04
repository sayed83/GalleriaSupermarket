/// <reference path="../jquery-3.3.1.min.js" />

$(document).ready(function () {
    shoppingCart.init();
});

(function () {
    this.shoppingCart = this.shoppingCart || {};
    var ns = this.shoppingCart;

    ns.init = function () {
        var cart = new ns.Cart();
        cart.GetItemsForSale();
        cart.loadCart();
        cart.calculateNetAmount();
        $('#Discount').change(function () {
            cart.calculateNetAmount();
        });

        $("#invTable").on("change", ".saleQnty", function () {
            var name = $(this).attr("data-name");
            var id = $(this).attr("data-itemid");
            var size = $(this).parents('tr').attr('data-size');
            var newQnty = parseFloat($(this).val());
            var ckQnty = parseInt($('#Qty'+id).text());
            if (newQnty > 0 && newQnty <= ckQnty) {
                cart.calculateQntyFromInput(name, size, newQnty);
                cart.calculateNetAmount();
            } else {
                if (newQnty <= 0) {
                    newQnty = 1;
                }else if(newQnty>ckQnty){
                    newQnty = ckQnty;
                }
                
                cart.calculateQntyFromInput(name, newQnty);
                cart.calculateNetAmount();
                $('.subBtn').attr('disabled', true);
                return false;
            }
            
        });
        $(document).on('click', '.addcart', cart.bindItemToCart);
        $('#orderSubmit').on('click', cart.submitOrder);
        $('#showSuccess').hide();
        $('#showError').hide();
        $(document).on('click', '.deleteBtn', cart.deleteItemFromBox);
        $(document).on('click', '.subBtn', cart.cartItemOrderRemove);
        $(document).on('click', '.addBtn', cart.cartItemOrder);
        $(document).on('change', '.SizeList', cart.SizeWisePriceCalculation);
    };

    ns.Cart = (function () {
        function Cart() { }//constructor
        var cart = [];

        function Item(id, name, size, price, count) {
            this.id = id;
            this.name = name;
            this.size = size;
            this.price = price;
            this.count = count;
        };

        // Save cart to the local Storage
        function saveCart() {
            localStorage.setItem("shoppingCart", JSON.stringify(cart));
        };
        //Load cart Funciton
        function loadCart() {
            cart = JSON.parse(localStorage.getItem("shoppingCart"));
        };
        loadCart();
        function addItemToCart(id, name, size, price, count) {
            for (var i in cart) {
                if (cart[i].id === id && cart[i].size == size) {
                    cart[i].count += count;
                    saveCart();
                    displayCart();
                    totalCartPrice();
                    return;
                }
            }
            var item = new Item(id, name, size, price, count);
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
        //count total Item
        function totalCountCart() {
            var totalCount = 0;
            for (var i in cart) {
                totalCount += cart[i].count;
            }
            return totalCount;
        };
        //single product totalPrice
        function perItemTotalPrice(id, size) {
            var totalCost = 0;
            for (var i in cart) {
                if (cart[i].id === id && cart[i].size == size) {
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

        function displayCart() {
            $('#GrossAmount').html('$' + totalCartPrice());
            var cartArray = listCart();
            var output = "";
            for (var i in cartArray) {
                output += "<tr class='item-row' id='" + cartArray[i].id + "' data-size='" + cartArray[i].size + "' data-id='" + cartArray[i].id + "' data-price='" + cartArray[i].price + "' data-count='" + cartArray[i].count + "' data-name='" + cartArray[i].name + "'><td><button type='button' data-size='" + cartArray[i].size + "' data-itemid='" + cartArray[i].id + "' class='btn btn-primary deleteBtn'>X</button></td><td>" + cartArray[i].name + "</td>";
                output += '<td>' + cartArray[i].size + '</td>';
                output += "<td>$" + cartArray[i].price + "</td><td><button type='button' class='btn btn-info calBtn subBtn' data-item='" + cartArray[i].id + "' data-size='" + cartArray[i].size + "'>-</button></td><td id='InvQty" + cartArray[i].id + "'><input type='text' data-itemid='" + cartArray[i].id + "' data-name='" + cartArray[i].name + "' id='saleQnty' class='form-control saleQnty' value='" + cartArray[i].count + "'/></td><td><button type='button' class='btn btn-info calBtn addBtn' data-size='" + cartArray[i].size + "' data-item='" + cartArray[i].id + "'>+</button></td><td class='TotalCost'>" + perItemTotalPrice(cartArray[i].id, cartArray[i].size) + "</td></tr>";
            }
            $('#invTable tbody').html(output);
        };

        Cart.prototype.calculateQntyFromInput = function (name, size, newQnty) {
            for (var i in cart) {
                if (cart[i].name === name && cart[i].size == size) {
                    cart[i].count = newQnty;
                    break;
                }
            }
            saveCart();
            displayCart();
        }

        Cart.prototype.GetItemsForSale = function () {
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
                        $this.empty();
                        var output;
                        var common = { id: 0, size: 'size' }
                        data.splice(0, 0, common);
                        $.each(data, function (i, r) {
                            output = '<option value="' + r.size + '">' + r.size + '</option>';
                            $this.append(output);
                        });
                    }
                });

            });
        };

        Cart.prototype.loadCart = function () {
            displayCart();
        };

        Cart.prototype.SizeWisePriceCalculation = function () {
            var $this = $(this);
            var oldVal = $this.val();
            var size = $this.val();
            $(this).parents('tr').find('.addcart').attr('data-size', size);
            size = parseFloat(size.replace('/g|kg|L|ml/gi', ''));//gi means req exp pattern caseinsensetive

            var price = $this.attr('data-price');
            price = parseFloat(price);

            if (size === 250) {
                $this.attr('data-actualprice', Number(price / 4));
                $this.parent().siblings('.info-product-price').find('.item_price').text('$' + Number(price / 4));
            } else if (size === 500) {
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

        Cart.prototype.bindItemToCart = function () {
            var itemid = parseInt($(this).attr("data-id"));
            var itemname = $(this).attr("data-name");
            var itemprice = parseFloat($(this).parents('tr').find('.SizeList').attr("data-actualprice"));
            var itemcount = parseInt($(this).attr("data-count"));
            var size = $(this).parents('tr').find('.SizeList').val();
            addItemToCart(itemid, itemname, size, itemprice, itemcount);
        };

        Cart.prototype.submitOrder = function () {
            var Products = [];
            var customerName = $('#Customer').val();
            var custPhone = $('#Contact').val();
            var payment = $('#Payment').val();
            var discount = $('#Discount').val();

            $("tr.item-row").each(function () {
                $this = $(this);
                var itemid = parseInt($this.attr("data-id"));
                var price = parseFloat($this.attr("data-price"));
                var qnty = parseInt($this.attr("data-count"));
                var size = $this.attr('data-size');
                Products.push({ ItemID: itemid, Price: price, OrderQnty: qnty, ItemSize:size });
            });
            if (payment == 0) {
                $('#showError').show();
            } else {
                $.ajax({
                    url: '/InternalSale/Create',
                    type: 'POST',
                    traditional: true,
                    data: { order: JSON.stringify(Products), custName: customerName, pmntType: payment, invDiscount: discount, phone: custPhone },
                    dataType: 'json',
                    success: function (res) {
                        $('#modal-table').modal('hide');
                        $('#GrossAmount').text('');
                        $('#Discount').val('');
                        $('#NetAmount').text('$0.00');
                        $('#showSuccess').show();
                        clearCart();
                        displayCart();
                    }
                });
            }


        };

        Cart.prototype.calculateNetAmount = function () {
            var grossamnt = $('#GrossAmount').text();
            grossamnt = parseFloat(grossamnt.replace('$', '0'));
            var discount = $('#Discount').val()||0;
            discount = parseFloat(discount);
            $('#NetAmount').text('$' + Number(grossamnt - discount));
        }

        Cart.prototype.cartItemOrder = function () {
            var id = parseInt($(this).attr("data-item"));
            var size = $(this).attr("data-size");
            addItemToCart(id, '',size, 0, 1);
            displayCart();
        };

        Cart.prototype.cartItemOrderRemove = function () {
            var id = $(this).attr("data-item");
            var size = $(this).attr("data-size");
            removeItemFromCart(id, size);
            displayCart();
        };

        Cart.prototype.deleteItemFromBox = function () {
            var id = $(this).attr("data-itemid");
            var size = $(this).attr("data-size");
            removeItemFromCartAll(id, size);
            displayCart();
        };


        return Cart;
    }());


})();


// Add to cart button
jQuery(function ($) {

    $("ul#productTab li a").on('shown.bs.tab', function (e) {
        var isTab = $(this).attr('href');
        var reviewTab = $('#review-tab').val();
        if (isTab == '#reviews' && reviewTab == 0) {
            $('#review-tab').val(1);
            loadReviewPagination();
        }
    });

    $(document).ready(function () {
        $(".review-pagination-list").eshopPagination({
            containerID: "wrap-review",
            perPage: 5,
        });
    });


    $(document).ready(function () {

        $("#cart-item").load("/Cart/_PartialCart");

    });


    jQuery(function ($) {
        $(document).delegate(".quantity, .spquantity", "change", function () {
            pid = $(this).attr("data-id");
            qty = $(this).val();
            $.ajax({
                url: "/Cart/Update",
                data: { id: pid, quantity: qty },
                success: function (response) {
                    $("#eshop-cart-total").text($.number(response.Count));
                    $("#Amount-" + pid).text($.number(response.Amount));
                    $(".nn-cart-total").text($.number(response.Total));
                    $("#cart-item").load("/Cart/_PartialCart");
                },
                complete: function () {
                    updateOrderSummary();
                }
            });
        });
    });

    jQuery(function ($) {
        $(document).delegate(".color, .spcolor", "change", function () {
            debugger
            pid = $(this).attr("data-id");
            var selectedOption = $(this).find("option:selected"); 
            var color = selectedOption.attr("value");
            debugger
            $.ajax({
                url: "/Cart/UpdateColor",
                data: { id: pid, color: color },
                success: function (response) {
                    $("#eshop-cart-total").text($.number(response.Count));
                    $("#Amount-" + pid).text($.number(response.Amount));
                    $(".nn-cart-total").text($.number(response.Total));
                    $("#cart-item").load("/Cart/_PartialCart");
                },
                complete: function () {
                    updateOrderSummary();
                }
            });
        });
    });
    jQuery(function ($) {
        $(document).delegate(".size, .spsize", "change", function () {
            debugger
            pid = $(this).attr("data-id");
            var selectedOption = $(this).find("option:selected");
            var size = selectedOption.attr("value");
            debugger
            $.ajax({
                url: "/Cart/UpdateSize",
                data: { id: pid, size: size },
                success: function (response) {
                    $("#eshop-cart-total").text($.number(response.Count));
                    $("#Amount-" + pid).text($.number(response.Amount));
                    $(".nn-cart-total").text($.number(response.Total));
                    $("#cart-item").load("/Cart/_PartialCart");
                },
                complete: function () {
                    updateOrderSummary();
                }
            });
        });
    });


});

function toast({ title = "", message = "", type = "info", duration = 500000 }) {
    const main = document.getElementById("toast_msg");
    if (main) {
        const toast = document.createElement("div");

        // // Auto remove toast
        const autoRemoveId = setTimeout(function () {
            main.removeChild(toast);
        }, duration + 1000);

        // Remove toast when clicked
        toast.onclick = function (e) {
            if (e.target.closest(".toast_msg__close")) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        };

        const icons = {
            success: "fas fa-check-circle",
            info: "fas fa-info-circle",
            warning: "fas fa-exclamation-circle",
            error: "fas fa-exclamation-circle"
        };
        const icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        toast.classList.add("toast_msg", `toast_msg--${type}`);
        toast.style.animation = `slideInLeft ease .3s, fadeOut linear 1s ${delay}s forwards`;

        toast.innerHTML = `
                      <div class="toast_msg__icon">
                          <i class="${icon}"></i>
                      </div>
                      <div class="toast_msg__body">
                          <h3 class="toast_msg__title">${title}</h3>
                          <p class="toast_msg__msg">${message}</p>
                      </div>
                      <div class="toast_msg__close">
                          <i class="fas fa-times"></i>
                      </div>
                  `;
        main.appendChild(toast);
    }
}

function ToastMessage(title, message, type) {
    toast({
        title: title,
        message: message,
        type: type,
        duration: 5000
    });
}



function deleteFromCart(pid) {
    var tr = jQuery(".row-" + pid);
    jQuery.ajax({
        url: "/Cart/Remove",
        data: { id: pid },
        beforeSend: function () {
            jQuery('.wait').html('<img src="Assets/Frontend/components/com_eshop/assets/images/loading.gif" alt="" />');
        },
        success: function (response) {
            jQuery('.wait').html('');
            jQuery("#eshop-cart-total").text(response.Count);

            jQuery("#cart-item").load("/Cart/_PartialCart");
            document.getElementById('row-' + pid).remove()
            //tr.hide(500);
        },
        complete: function () {
            updateOrderSummary();
        }
    });
    return false;
}

function flyToCart(pid) {
    var ty = jQuery("#addToCart").closest('.product').find('#' + pid);
    var img = jQuery("#product-image-" + pid);
    quatity = jQuery('#quantity_' + pid).val();
    var color = null;
    var size = null;
    if (document.getElementsByClassName("colorButton").length == 0) {
        color = "default";
    }
    else {
        if (selectedColor === "undefined") {
            alert("Bạn Chưa Chọn Màu!");
        }
        else {
            color = selectedColor;
        }
    }
    if (document.getElementsByClassName("sizeButton").length == 0) {
        size = "default";
    }
    else
    {
        if (selectedSize === "undefined") {
            alert("Bạn Chưa Chọn Size!");
        }
        else {
            size = selectedSize;
        }
    }

    if (quatity == 'undefined' || quatity == null) {
        quatity = 1;
    }
    
    jQuery.ajax({
        type: 'GET', 
        dataType: 'json',
        url: 'Cart/Add',
        data: { id: pid, quatity: quatity, color: color, size: size },
        beforeSend: function () {
            jQuery('.add-to-cart').attr('disabled', true);
            jQuery('.add-to-cart').after('<span class="wait">&nbsp;<img src="Assets/Frontend/components/com_eshop/assets/images/loading.gif" /></span>');
        },
        complete: function () {
            jQuery('.add-to-cart').attr('disabled', false);
            jQuery('.wait').remove();
        },
        success: function (result) {
            //alert(result.Count);
            jQuery("#eshop-cart-total").text(result.Count);
        }
    }).done(function (response) {
        //Load giỏ hàng
        jQuery("#cart-item").load("/Cart/_PartialCart")

        flyToElement(jQuery(img), jQuery('#eshop-cart'));

        /*jQuery("html, body").animate({ scrollTop: 0 }, 2000);*/

        //Mở giỏ hàng
        //jQuery('.eshop-content').slideToggle();
        alert("Thêm Sản Phẩm Thành Công");

        //Đóng giỏ hàng sau 8s
        setTimeout(function () {
            //jQuery('.eshop-content').hide();
        }, 8000);

    }).fail(function () {
        alert("fail");
    });

    return false;
}

function flyToCart1(pid) {
    var ty = jQuery("#addToCart").closest('.product').find('#' + pid);
    var img = jQuery("#product-image-" + pid);
    quatity = jQuery('#quantity_' + pid).val();
    var color = null;
    var size = null;
    if (document.getElementsByClassName("colorButton").length == 0) {
        color = "default";
    }
    else {
        if (selectedColor === "undefined") {
            alert("Bạn Chưa Chọn Màu!");
        }
        else {
            color = selectedColor.innerHTML;
        }
    }
    if (document.getElementsByClassName("sizeButton").length == 0) {
        size = "default";
    }
    else {
        if (selectedSize === "undefined") {
            alert("Bạn Chưa Chọn Size!");
        }
        else {
            size = selectedSize.innerHTML;
        }
    }

    if (quatity == 'undefined' || quatity == null) {
        quatity = 1;
    }

    jQuery.ajax({
        type: 'GET',
        dataType: 'json',
        url: 'Cart/Add',
        data: { id: pid, quatity: quatity, color: color, size: size },
        beforeSend: function () {
            jQuery('.add-to-cart').attr('disabled', true);
            jQuery('.add-to-cart').after('<span class="wait">&nbsp;<img src="Assets/Frontend/components/com_eshop/assets/images/loading.gif" /></span>');
        },
        complete: function () {
            jQuery('.add-to-cart').attr('disabled', false);
            jQuery('.wait').remove();
        },
        success: function (result) {
            //alert(result.Count);
            jQuery("#eshop-cart-total").text(result.Count);
        }
    }).done(function (response) {
        //Load giỏ hàng
        jQuery("#cart-item").load("/Cart/_PartialCart")

        flyToElement(jQuery(img), jQuery('#eshop-cart'));

        /*jQuery("html, body").animate({ scrollTop: 0 }, 2000);*/

        //Mở giỏ hàng
        //jQuery('.eshop-content').slideToggle();
        alert("Thêm Sản Phẩm Thành Công");

        //Đóng giỏ hàng sau 8s
        setTimeout(function () {
            //jQuery('.eshop-content').hide();
        }, 8000);

    }).fail(function () {
        alert("fail");
    });

    return false;
}
function format1(n,) {
    return   n.toFixed(2).replace(/./g, function (c, i, a) {
        return i > 0 && c !== "." && (a.length - i) % 3 === 0 ? "," + c : c;
    });
}
function formatMoney(number, decPlaces, decSep, thouSep) {
    decPlaces = isNaN(decPlaces = Math.abs(decPlaces)) ? 2 : decPlaces,
        decSep = typeof decSep === "undefined" ? "." : decSep;
    thouSep = typeof thouSep === "undefined" ? "," : thouSep;
    var sign = number < 0 ? "-" : "";
    var i = String(parseInt(number = Math.abs(Number(number) || 0).toFixed(decPlaces)));
    var j = (j = i.length) > 3 ? j % 3 : 0;

    return sign +
        (j ? i.substr(0, j) + thouSep : "") +
        i.substr(j).replace(/(\decSep{3})(?=\decSep)/g, "$1" + thouSep) +
        (decPlaces ? decSep + Math.abs(number - i).toFixed(decPlaces).slice(2) : "");
}

function updateOrderSummary() {

    jQuery.ajax({
        url: '/Order/getOrderInfo',
        type: 'Get',
        dataType: 'json',
        success: function (response) {
            if (response.OrderTotal == 0) {
                window.location.href = '/Home/Index';
            }
            jQuery("#orderTransportCost").text(formatMoney(response.TransportCost, ".", ",") + "đ");
            jQuery("#orderDiscount").text(response.Discount + "đ");
            jQuery("#totalOrder").text(response.OrderTotal + "đ");
            jQuery("#discountDescription").html(response.DiscountDescription);
        },
        error: function (err) {
            alert("Lỗi hệ thống, ấn F5 để refresh lại trình duyệt để tiếp tục.");
        }
    });
}



function flyToElement(flyer, flyingTo, callBack /*callback is optional*/) {

    var func = jQuery(this);

    var divider = 3;

    var flyerClone = jQuery(flyer).clone();

    jQuery(flyerClone).css({
        position: 'absolute',
        width: '80px',
        height: 'auto',
        top: (jQuery(flyer).offset().top + (jQuery(flyer).width() / 2)) + "px",
        left: (jQuery(flyer).offset().left + (jQuery(flyer).height() / 2)) + "px",
        opacity: 1,
        'z-index': 1000
    });

    jQuery('body').append(jQuery(flyerClone));

    var gotoX = jQuery(flyingTo).offset().left + (jQuery(flyingTo).width() / 2) - (jQuery(flyerClone).width() / divider) / 2;
    var gotoY = jQuery(flyingTo).offset().top + (jQuery(flyingTo).height() / 2) - (jQuery(flyerClone).height() / divider) / 2;

    jQuery(flyerClone).animate({
        opacity: 0.4,
        left: gotoX,
        top: gotoY,
        width: jQuery(flyerClone).width() / divider,
        height: jQuery(flyerClone).height() / divider
    }, 700,
    function () {
        jQuery(flyingTo).fadeOut('fast', function () {
            jQuery(flyingTo).fadeIn('fast', function () {
                jQuery(flyerClone).fadeOut('fast', function () {
                    jQuery(flyerClone).remove();
                    if (callBack != null) {
                        callBack.apply(func);
                    }
                });
            });
        });
    });

    return false;
}

function flyFromElement(flyer, flyingTo, callBack /*callback is optional*/) {
    var func = jQuery(this);

    var divider = 3;

    var beginAtX = jQuery(flyingTo).offset().left + (jQuery(flyingTo).width() / 2) - (jQuery(flyer).width() / divider) / 2;
    var beginAtY = jQuery(flyingTo).offset().top + (jQuery(flyingTo).width() / 2) - (jQuery(flyer).height() / divider) / 2;

    var gotoX = jQuery(flyer).offset().left;
    var gotoY = jQuery(flyer).offset().top;

    var flyerClone = jQuery(flyer).clone();

    jQuery(flyerClone).css({
        position: 'absolute',
        top: beginAtY + "px",
        left: beginAtX + "px",
        opacity: 0.4,
        'z-index': 1000,
        width: jQuery(flyer).width() / divider,
        height: jQuery(flyer).height() / divider
    });
    jQuery('body').append(jQuery(flyerClone));

    jQuery(flyerClone).animate({
        opacity: 1,
        left: gotoX,
        top: gotoY,
        width: jQuery(flyer).width(),
        height: jQuery(flyer).height()
    }, 700,
    function () {
        jQuery(flyerClone).remove();
        jQuery(flyer).fadeOut('fast', function () {
            jQuery(flyer).fadeIn('fast', function () {
                if (callBack != null) {
                    callBack.apply(func);
                }
            });
        });
    });

    return false;
}

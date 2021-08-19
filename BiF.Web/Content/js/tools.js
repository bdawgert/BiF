﻿
function writeError(message) {
	console.debug(message);
}

$(function() {

	$('.navigation-hamberder').on('click', function (e) {
		console.log('click');
		e.stopPropagation();
		$('.navigation-bar').toggle();
	});

	$(document).on('click', function (e) {
		console.log('click');
		e.stopPropagation();

		if ($('.navigation-hamberder').is(':visible') && $('.navigation-bar').is(':visible'))
			$('.navigation-bar').hide();
	});
});


var Popup = function() {
	this._popup = $('.popup');
	this.Show = function(msg) {
		$('.overlay').show().css('display', 'flex');

		if (msg !== '')
			this._popup.html(msg);
		this._popup.show();
	};

	this.Hide = function() {
		$('.overlay').hide();
		this._popup.hide();
	};

};

var popup;

$(function() {
	popup = new Popup();
});


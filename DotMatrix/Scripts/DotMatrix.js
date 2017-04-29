function postJson(url, vars, callback, errorcallback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "POST",
		dataType: 'json',
		data: vars,
		success: function (response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (errorcallback) {
				errorcallback(jqXHR, textStatus, errorThrown);
			}
		}
	});
}

function getJson(url, vars, callback, errorcallback) {
	$.ajax({
		url: url,
		cache: false,
		async: true,
		type: "GET",
		dataType: 'json',
		data: vars,
		success: function (response, textStatus, jqXHR) {
			if (callback) {
				callback(response);
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (errorcallback) {
				errorcallback(jqXHR, textStatus, errorThrown);
			}
		}
	});
}

function getMousePos(canvas, evt) {
	var rect = canvas.getBoundingClientRect();
	return {
		x: evt.clientX - rect.left,
		y: evt.clientY - rect.top
	};
}

function rgb2hex(red, green, blue) {
	var rgb = blue | (green << 8) | (red << 16);
	return '#' + (0x1000000 + rgb).toString(16).slice(1)
}

function hex2rgb(hex) {
	// long version
	r = hex.match(/^#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})$/i);
	if (r) {
		return r.slice(1, 4).map(function (x) { return parseInt(x, 16); });
	}
	// short version
	r = hex.match(/^#([0-9a-f])([0-9a-f])([0-9a-f])$/i);
	if (r) {
		return r.slice(1, 4).map(function (x) { return 0x11 * parseInt(x, 16); });
	}
	return null;
}
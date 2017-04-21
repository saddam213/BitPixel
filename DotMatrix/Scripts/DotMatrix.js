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
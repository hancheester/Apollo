$(function () {
    $('#global-search').on('click', function () {
        var filter = $('#search-filter').val();
        var query = $('#search-query').val();
        
        submitForm('/search.aspx', { Filter: filter, Query: query });
    });
});

$('.spin-icon').click(function () {
    $('.search-float-box').toggleClass('show');
});

function submitForm(path, params, method) {
    method = method || "post"; // Set method to post by default if not specified.
    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
}
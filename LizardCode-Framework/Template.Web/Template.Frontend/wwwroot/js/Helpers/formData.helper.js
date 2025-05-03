var FormDataEx = (function () {
    var self = this;
    var _formData;

    function Person(formElement, jsonToAppend) {
        _formData = new FormData();

        this.Parse(formElement);
        this.AppendJson(jsonToAppend);
    }


    Person.prototype.Parse = function (formElement) {

        if (formElement === undefined || formElement.tagName.toLowerCase() != 'form')
            return this;

        _formData = new FormData(formElement);

        return this;
    };

    Person.prototype.Append = function (name, value) {

        _formData.append(name, value);

        return this;
    }

    Person.prototype.AppendJson = function (json) {

        appendFormData(_formData, json);

        return this;
    }

    Person.prototype.Serialize = function () {

        return new URLSearchParams(_formData).toString();

    }

    //
    // https://stackoverflow.com/a/49388446/1812392
    //
    function appendFormData(formData, data, root) {
        root = root || '';
        if (data instanceof File) {
            formData.append(root, data);
        }
        else if (Array.isArray(data)) {
            for (var i = 0; i < data.length; i++) {
                appendFormData(formData, data[i], root + '[' + i + ']');
            }
        }
        else if (typeof data === 'object' && data) {
            for (var key in data) {
                if (data.hasOwnProperty(key)) {
                    if (root === '') {
                        appendFormData(formData, data[key], key);
                    } else {
                        appendFormData(formData, data[key], root + '.' + key);
                    }
                }
            }
        }
        else {
            if (data !== null && typeof data !== 'undefined') {
                formData.append(root, data);
            }
        }
    }

    return Person;
})();


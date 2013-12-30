(function ($) {

    var FileTransfer = function(fileName, size) {
        this.fileName = fileName;
        this.size = size;
        this.progress = ko.observable(0);
        this.label = ko.observable();
        this.status = ko.observable("uploading");
        this.uploadedFileName = ko.observable();
    };

    var FileUploadViewModel = function () {
        var self = this;
        this.transfers = ko.observableArray([]);
        this.upload = function(file) {
            var transfer = new FileTransfer(file.name, file.size);
            this.transfers.push(transfer);
            return transfer;
        };
        this.remove = function() {
            self.transfers.remove(this);
        };
    };

    $(function () {
        var fileUpload = $("#file-upload");
        var viewModel = new FileUploadViewModel();
        ko.applyBindings(viewModel, $("#file-upload")[0]);

        $("#browse-files").on("click", function () {
            fileUpload.find("input").trigger("click");
        });

        fileUpload.find("input").on("click", function (event) {
            event.stopPropagation();
        });

        fileUpload.fileupload({
            url: fileUpload.data("upload-url"),
            autoUpload: true,
            formData: {
                __RequestVerificationToken: fileUpload.data("anti-forgery-token")
            },
            progressall: function (e, data) { },
            add: function (e, data) {
                if (!data.files.length) {
                    return;
                }

                var filesLength = data.files.length;
                for (var i = 0; i < filesLength; i++) {
                    var file = data.files[i];
                    data.context = viewModel.upload(file);
                }

                data.submit();
            },
            progress: function (e, data) {
                if (data.context) {
                    var progress = Math.floor(data.loaded / data.total * 100);
                    data.context.progress(progress);
                    data.context.label(data.context.fileName + ' - ' + progress + '%');
                }
            },
            pasteZone: fileUpload[0],
            paste: function (e, data) {
                return true;
            },
            done: function (e, data) {
                var result = data.result;
                data.context.status("success");
                data.context.uploadedFileName(result.uploadedFileName);
                data.context.label(data.context.fileName);
            },
            fail: function (e, data) {
                data.context.status("error");
            }
        });
    });
})(jQuery);
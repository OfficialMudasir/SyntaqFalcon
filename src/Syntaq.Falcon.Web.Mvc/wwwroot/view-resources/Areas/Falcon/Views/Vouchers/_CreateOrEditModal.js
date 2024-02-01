(function ($) {
    app.modals.CreateOrEditVoucherModal = function () {

        var _vouchersService = abp.services.app.vouchers;
        var _modalManager;
        var _$voucherInformationForm = null;

        var _$EntityLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Vouchers/EntityLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Vouchers/_EntityLookupTableModal.js',
            modalClass: 'EntityLookupTableModal'
        });



        $('#DiscountType').change(function () {
            ;
            voucherValueSymbol(this);
        });

        function voucherValueSymbol(DiscType) {
            $("#voucherValueSymbol").text($(DiscType).val() == "Choose" ? "" : $(DiscType).val() == "Fixed" ? "$" : "%" ); 
        }

        $('#RestrictVoucherCheck').change(function () {
            if ($(this).is(":checked")) {
                showHiddenSection();

                $('.vEntity_vID').val(fill_VouchID());

                ;

                if (vEntitiesCount == "0") {
                    cloneAppendTemplate();
                }

                //if (JSONIsEditMode == false) {
                //    cloneAppendTemplate();
                //} else {
                //    if (vEntitiesCount == 0) {
                //        cloneAppendTemplate();
                //    }
                //}

            } else {
                hideHiddenSections();
            }
        });

        function showHiddenSection() {
            $("#voucherEntity_formblock").show();
            $("#HiddenEntitySection").show();
            $("#HiddenEntitySectionBtn").show();
        }

        function hideHiddenSections() {
            $("#voucherEntity_formblock").hide();
            $("#HiddenEntitySection").hide();
            $("#HiddenEntitySectionBtn").hide();
        }

        $('#addEntityBtn').click(function () {
            ;
            cloneAppendTemplate();
        })

        function cloneAppendTemplate() {
            var clone = $('#EntityGroup_Template').clone();
            clone.removeAttr("id");

            setLookupModalEvents(clone);

            $('#voucherEntity_formblock').append(clone);

            $('.vEntity_vID').val(fill_VouchID());
        }

        function fill_VouchID() {
            ;
            //use jquery selector to fetch value of voucher_ID field into variable
            var vouchID = $('#voucher_ID').val();
            //set cloned ID to = variable
            return vouchID;
        }

        function uuidv4() {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        }

        //function getEntityGuid() {
        //    $('#Entity_ID').val(uuidv4());
        //}

        this.init = function (modalManager) {
            ;

            _modalManager = modalManager;
            ;

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$voucherInformationForm = _modalManager.getModal().find('form[name=VoucherInformationsForm]');
            _$voucherInformationForm.validate();

            if ($('#DiscountType').val() != "Choose") {
                $('#DiscountType').trigger('change');
            };

            //if (vEntitiesCount == 0) {
            //    $('#RestrictVoucherCheck').is("");
            //    $('#RestrictVoucherCheck').trigger('change');
            //}
            //else {
            //    $('#RestrictVoucherCheck').is(":checked");
            //    $('#RestrictVoucherCheck').trigger('change');
            //}

            $('#RestrictVoucherCheck').trigger('change');


            if (JSONIsEditMode == false) {
                $('#voucher_ID').val(uuidv4());
            }
        };

        var _selectedEntity;
        function setLookupModalEvents(context) {
            $('.OpenEntityLookupTableButton', context).click(function () {

                ;
                _selectedEntity = $(this).closest('.formblock');
                _$EntityLookupTableModal.open({ id: '9E372214-B1D3-47CC-48F6-08D62F2A232A', displayName: '' },
                    function (data) {
                        $('[name="Entity[][EntityName]"]', _selectedEntity).val(data.displayName);
                        $('[name="Entity[][EntityKey]"]', _selectedEntity).val(data.id);
                        $('[name = "Entity[][EntityType]"]', _selectedEntity).val("Form");
                    });
            });

            //modal event for voucher usage???
        }
        setLookupModalEvents(null);


        this.save = function () {
            ;

            if (!_$voucherInformationForm.valid()) {
                return;
            }

            ;

            var voucherEntities = $('#voucherEntity_formblock').serializeJSON({ useIntKeysAsArrayIndex: false });
            ;

            var voucher = {
                ID: $('[name=voucherId]').val(),
                Key: $('[name=Key]').val(),
                Value: $('[name=Value]').val(),
                Expiry: $('[name=Expiry]').val(),
                NoOfUses: $('[name=NoOfUses]').val(),
                Description: $('[name=Description]').val(),
                DiscountType: $('[name=DiscountType]').val(),
                VoucherEntities: voucherEntities.Entity
            };
            ;
			 _modalManager.setBusy(true);
            _vouchersService.createOrEdit(
                voucher
            ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditVoucherModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
    };
})(jQuery);





(function ($) {
	app.modals.ManageACLModal = function () {
		var _aclService = abp.services.app.aCLs;
		var _userService = abp.services.app.user;
		var _organizationUnitService = abp.services.app.organizationUnit;
		var _modalManager;
		var _$form = null;

		this.init = async function (modalManager) {
			_modalManager = modalManager;
			_$modalDialog = _modalManager.getModal().find('.modal-dialog');
			_$modalDialog.css("max-width", "43em");
			_$form = _modalManager.getModal().find('form');
			_$form.validate();

			var _localSource = [];

			var users = await _userService.getUsers({ filter: null, permission: null, role: null, onlyLockedUsers: false, maxResultCount: 999 });
			//var users = await _userService.getUsersForSharing({ filter: null, permission: null, role: null, onlyLockedUsers: false });
			
			$(users.items).each(function () {
				var user = { display: "" + this.userName + "", type: "User", id: this.id };
				_localSource.push(user);
			});

			var HasFeature = abp.session.tenantId === null ? true : abp.features.isEnabled('App.TeamsManagement');
			if (HasFeature) {

				//var teams = await _organizationUnitService.getOrganizationUnitsForSharing();
				var teams = await _organizationUnitService.getOrganizationUnits();
				$(teams.items).each(function () {
					var team = { display: "" + this.displayName + "", type: "Team", id: this.id };
					_localSource.push(team);
				});
			}

			var assignees = new Bloodhound({
				datumTokenizer: Bloodhound.tokenizers.obj.whitespace('display'),
				queryTokenizer: Bloodhound.tokenizers.whitespace,
				local: _localSource,
			});

			assignees.initialize();
			var elt = $('.tagsinput-typeahead');

			elt.tagsinput({
				itemValue: 'display',
				itemText: 'display',
				typeaheadjs: {
					name: 'assignees',
					displayKey: 'display',
					//source: assignees.ttAdapter(),
					source: function (query, sync, async) {
						var matches = $.map(_localSource, function (user) {
							return Bloodhound.tokenizers.whitespace(query).some(function (token) {
								return user.display.toLowerCase().includes(token.toLowerCase());
							}) ? user : null;
						});
						sync(matches);
					}
				}
			});
		};

		async function grantACL() {
			//var ACL = _$form.serializeFormToObject();
			//await _aclService.grantACL(ACL)
			var ACLForm = _$form.serializeFormToObject();
			ACLForm.Assignees = $(".tagsinput-typeahead").tagsinput('items');
			await _aclService.grantACL(ACLForm);
		};

		function modifyACL(ACLId, Role) {
			var ACL = {};
			ACL.ACLId = ACLId;
			ACL.Type = false;
			ACL.Username = null;
			ACL.UserId = null;
			ACL.OrgId = null;
			ACL.Role = Role;
			ACL.EntityId = "00000000-0000-0000-0000-000000000000";

			_aclService.updateACL(ACL);
		};

		function revokeACL(ACLId) {
			var ACL = {};
			ACL.ACLId = ACLId;
			ACL.Type = false;
			ACL.Username = null;
			ACL.UserId = null;
			ACL.OrgId = null;
			ACL.Role = null;
			ACL.EntityId = "00000000-0000-0000-0000-000000000000";

			_aclService.revokeACL(ACL);
		};

		$('#GrantACL').click(async function () {
			await grantACL();
			abp.notify.info(app.localize('Saved'));
			_modalManager.close({ refreshPageAfterClose: false });
		});

		$('#UpdateACL').click(function (event, ACLId, Role) {
			modifyACL(ACLId, Role);
			abp.notify.info(app.localize('Saved'));
			_modalManager.close({ refreshPageAfterClose: false });
		});

		$('#RevokeACL').click(function (event, ACLId) {
			revokeACL(ACLId);
			abp.notify.info(app.localize('Deleted'));
			_modalManager.close({ refreshPageAfterClose: false });
			//window.location.reload();
		});
	};
})(jQuery);
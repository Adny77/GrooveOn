import 'dart:io';

import 'package:flutter/material.dart';
import 'package:grooveon_mobile/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_mobile/dialogs/forgot_password_dialog.dart';
import 'package:grooveon_mobile/helper/image_helper.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/models/user.dart';
import 'package:grooveon_mobile/providers/image_provider.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:grooveon_mobile/validation/validation_model/validation_rules.dart';
import 'package:grooveon_mobile/validation/validation_use/universal_validator.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

class UserScreen extends StatefulWidget {
  const UserScreen({super.key});

  @override
  State<UserScreen> createState() => UserScreenState();
}

class UserScreenState extends State<UserScreen> {
  static const Color primary = Color(0xFF9C27B0);
  static const Color primaryDark = Color(0xFF6A1B9A);
  static const Color bg = Color(0xFFF8F6FB);

  final _firstName = TextEditingController();
  final _lastName = TextEditingController();
  final _username = TextEditingController();
  final _email = TextEditingController();
  final _phone = TextEditingController();
  final _birthDate = TextEditingController();

  final Map<String, String?> _errors = {};

  File? _pickedImage;
  bool _isImageChanged = false;

  User? _user;
  bool _syncingFields = false;
  String _savedFirstName = "";
  String _savedLastName = "";
  String _savedUsername = "";
  String _savedEmail = "";
  String _savedPhone = "";
  String _savedBirthDate = "";

  bool _loading = true;
  bool _saving = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadUser();

    for (final c in [
      _firstName,
      _lastName,
      _username,
      _email,
      _phone,
      _birthDate,
    ]) {
      c.addListener(_onFieldChanged);
    }
  }

  void _onFieldChanged() {
    if (_syncingFields || !mounted) return;

    setState(() {
      if (_errors.isNotEmpty) {
        _errors.clear();
      }
    });
  }

  @override
  void dispose() {
    _firstName.dispose();
    _lastName.dispose();
    _username.dispose();
    _email.dispose();
    _phone.dispose();
    _birthDate.dispose();
    super.dispose();
  }

  Future<bool> confirmLeaveScreen() async {
    return await _confirmExitIfChanged();
  }

  Future<bool> _confirmExitIfChanged() async {
    if (!_hasChanges()) return true;

    final shouldLeave = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Unsaved changes",
      question:
          "You have unsaved changes.\n\nAre you sure you want to leave? All changes will be lost.",
      yesText: "Leave",
      noText: "Stay",
      danger: true,
    );

    if (shouldLeave) {
      setState(() {
        _pickedImage = null;
        _isImageChanged = false;
        _errors.clear();
      });

      await _loadUser();
    }

    return shouldLeave;
  }

  bool _hasChanges() {
    if (_user == null) return false;

    return _firstName.text.trim() != _savedFirstName ||
        _lastName.text.trim() != _savedLastName ||
        _username.text.trim() != _savedUsername ||
        _email.text.trim() != _savedEmail ||
        _phone.text.trim() != _savedPhone ||
        _birthDate.text.trim() != _savedBirthDate ||
        _isImageChanged;
  }

  void _setSavedSnapshot(User user) {
    _savedFirstName = user.firstName;
    _savedLastName = user.lastName;
    _savedUsername = user.username;
    _savedEmail = user.email;
    _savedPhone = user.phoneNumber ?? "";
    _savedBirthDate = user.dateOfBirth == null
        ? ""
        : _formatDate(user.dateOfBirth!);
  }

  void _markCurrentValuesSaved() {
    _savedFirstName = _firstName.text.trim();
    _savedLastName = _lastName.text.trim();
    _savedUsername = _username.text.trim();
    _savedEmail = _email.text.trim();
    _savedPhone = _phone.text.trim();
    _savedBirthDate = _birthDate.text.trim();
    _pickedImage = null;
    _isImageChanged = false;
    _errors.clear();
  }

  Future<void> _pickImage() async {
    final picker = ImagePicker();

    final picked = await picker.pickImage(
      source: ImageSource.gallery,
      imageQuality: 85,
    );

    if (picked == null) return;

    setState(() {
      _pickedImage = File(picked.path);
      _isImageChanged = true;
    });
  }

  Future<void> _loadUser() async {
    try {
      setState(() {
        _loading = true;
        _error = null;
      });

      final userId = Session.userId;
      if (userId == null) throw Exception("User is not logged in.");

      final user = await context.read<UserProvider>().getById(userId);

      _syncingFields = true;
      _user = user;
      _setSavedSnapshot(user);
      _firstName.text = _savedFirstName;
      _lastName.text = _savedLastName;
      _username.text = _savedUsername;
      _email.text = _savedEmail;
      _phone.text = _savedPhone;
      _birthDate.text = _savedBirthDate;
      _pickedImage = null;
      _isImageChanged = false;
      _syncingFields = false;

      if (!mounted) return;
      setState(() => _loading = false);
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.toString();
        _loading = false;
      });
    }
  }

  bool _validateProfile() {
    _errors.clear();

    final ok = ValidationEngine.validate([
      Rules.requiredText(
        "firstName",
        _firstName.text,
        "First name is required.",
      ),
      Rules.requiredText("lastName", _lastName.text, "Last name is required."),
      Rules.username("username", _username.text),
      Rules.email("email", _email.text),
      Rules.phone("phoneNumber", _phone.text, required: true),
      Rules.requiredText(
        "birthDate",
        _birthDate.text,
        "Date of birth is required.",
      ),
    ], (field, message) => _errors[field] = message);

    setState(() {});
    return ok;
  }

  Future<void> _save() async {
    if (!_hasChanges()) return;
    if (!_validateProfile()) return;

    final userId = Session.userId;
    if (userId == null || _user == null) return;

    setState(() => _saving = true);

    try {
      String? finalImage = _user?.userImage;

      if (_pickedImage != null) {
        final uploadedFileName = await ImageAppProvider.upload(
          file: _pickedImage!,
          folder: "users",
        );

        finalImage = uploadedFileName;
      }

      await context.read<UserProvider>().update(userId, {
        "firstName": _firstName.text.trim(),
        "lastName": _lastName.text.trim(),
        "username": _username.text.trim(),
        "email": _email.text.trim(),
        "phoneNumber": _phone.text.trim(),
        "dateOfBirth": _toIsoDate(_birthDate.text.trim()),
        "userImage": finalImage,
        "isActive": _user?.isActive ?? true,
      });

      if (!mounted) return;

      setState(_markCurrentValuesSaved);

      SnackbarHelper.showUpdate(context, "Profile updated successfully.");

      await _loadUser();
    } catch (e) {
      if (!mounted) return;
      SnackbarHelper.showError(context, e.toString());
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _openChangePasswordDialog() async {
    final changed = await showDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (_) => const _ChangePasswordDialog(),
    );

    if (changed == true && mounted) {
      SnackbarHelper.showSuccess(context, "Password changed successfully.");
    }
  }

  Future<void> _openResetViaEmailDialog() async {
    await showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => const ForgotPasswordDialog(),
    );
  }

  Future<void> _logout() async {
    final canLeave = await _confirmExitIfChanged();
    if (!canLeave) return;

    Session.logout();
    if (!mounted) return;
    Navigator.of(context).pushNamedAndRemoveUntil("/login", (_) => false);
  }

  @override
  Widget build(BuildContext context) {
    if (_loading) {
      return const Scaffold(
        backgroundColor: bg,
        body: Center(child: CircularProgressIndicator(color: primary)),
      );
    }

    if (_error != null) {
      return Scaffold(
        backgroundColor: bg,
        body: Center(
          child: ElevatedButton(
            onPressed: _loadUser,
            child: const Text("Try again"),
          ),
        ),
      );
    }

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) async {
        if (didPop) return;

        final canLeave = await _confirmExitIfChanged();

        if (canLeave && mounted) {
          Navigator.of(context).pop();
        }
      },
      child: Scaffold(
        backgroundColor: bg,
        body: SafeArea(
          child: ListView(
            padding: const EdgeInsets.fromLTRB(18, 18, 18, 110),
            children: [
              _header(),
              const SizedBox(height: 16),
              _profileForm(),
              const SizedBox(height: 16),
              _securityCard(),
              const SizedBox(height: 16),
              SizedBox(
                height: 50,
                child: ElevatedButton.icon(
                  onPressed: _logout,
                  icon: const Icon(Icons.logout_rounded),
                  label: const Text(
                    "Log out",
                    style: TextStyle(fontWeight: FontWeight.w900),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.black87,
                    foregroundColor: Colors.white,
                    elevation: 0,
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(16),
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _header() {
    final name = "${_firstName.text} ${_lastName.text}".trim();
    final username = _username.text.isEmpty ? "GrooveOn user" : _username.text;

    return Container(
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [primaryDark, primary],
          begin: Alignment.topLeft,
          end: Alignment.bottomRight,
        ),
        borderRadius: BorderRadius.circular(24),
      ),
      child: Row(
        children: [
          Stack(
            children: [
              Container(
                width: 78,
                height: 78,
                decoration: BoxDecoration(
                  color: Colors.white24,
                  borderRadius: BorderRadius.circular(24),
                  border: Border.all(color: Colors.white.withOpacity(0.25)),
                ),
                child: ClipRRect(
                  borderRadius: BorderRadius.circular(22),
                  child: _pickedImage != null
                      ? Image.file(_pickedImage!, fit: BoxFit.cover)
                      : (_user?.userImage != null &&
                              _user!.userImage!.isNotEmpty)
                          ? Image.network(
                              _user!.userImage!,
                              fit: BoxFit.cover,
                              errorBuilder: (_, __, ___) => Center(
                                child: Text(
                                  username[0].toUpperCase(),
                                  style: const TextStyle(
                                    color: Colors.white,
                                    fontSize: 28,
                                    fontWeight: FontWeight.w900,
                                  ),
                                ),
                              ),
                            )
                          : Center(
                              child: Text(
                                username[0].toUpperCase(),
                                style: const TextStyle(
                                  color: Colors.white,
                                  fontSize: 28,
                                  fontWeight: FontWeight.w900,
                                ),
                              ),
                            ),
                ),
              ),
              Positioned(
                right: 0,
                bottom: 0,
                child: InkWell(
                  onTap: _pickImage,
                  borderRadius: BorderRadius.circular(14),
                  child: Container(
                    width: 34,
                    height: 34,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(14),
                      boxShadow: const [
                        BoxShadow(
                          color: Color(0x22000000),
                          blurRadius: 8,
                          offset: Offset(0, 4),
                        ),
                      ],
                    ),
                    child: const Icon(
                      Icons.camera_alt_outlined,
                      color: primary,
                      size: 18,
                    ),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  name.isEmpty ? "User" : name,
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 21,
                    fontWeight: FontWeight.w900,
                  ),
                ),
                Text(
                  "@$username",
                  style: const TextStyle(
                    color: Colors.white70,
                    fontWeight: FontWeight.w700,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _profileForm() {
    final hasChanges = _hasChanges();

    return _card(
      icon: Icons.person_outline_rounded,
      title: "Profile",
      subtitle: "Edit your basic account information.",
      child: Column(
        children: [
          Row(
            children: [
              Expanded(
                child: _field(
                  "First name",
                  Icons.badge_outlined,
                  _firstName,
                  errorKey: "firstName",
                ),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: _field(
                  "Last name",
                  Icons.badge_outlined,
                  _lastName,
                  errorKey: "lastName",
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          _field(
            "Username",
            Icons.alternate_email_rounded,
            _username,
            errorKey: "username",
          ),
          const SizedBox(height: 12),
          _field(
            "Email",
            Icons.mail_outline_rounded,
            _email,
            errorKey: "email",
            keyboardType: TextInputType.emailAddress,
          ),
          const SizedBox(height: 12),
          _field(
            "Phone",
            Icons.phone_outlined,
            _phone,
            errorKey: "phoneNumber",
            keyboardType: TextInputType.phone,
          ),
          const SizedBox(height: 12),
          _dateField(),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            height: 48,
            child: ElevatedButton(
              onPressed: _saving || !hasChanges ? null : _save,
              style: ElevatedButton.styleFrom(
                backgroundColor: primary,
                foregroundColor: Colors.white,
                disabledBackgroundColor: const Color(0xFFE9E1EE),
                elevation: 0,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
              child: _saving
                  ? const SizedBox(
                      width: 18,
                      height: 18,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text(
                      "Save changes",
                      style: TextStyle(fontWeight: FontWeight.w900),
                    ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _securityCard() {
    return _card(
      icon: Icons.lock_outline_rounded,
      title: "Security",
      subtitle: "Manage your account password.",
      child: Column(
        children: [
          _securityOption(
            icon: Icons.password_rounded,
            title: "Change password",
            description: "Use your current password to set a new one.",
            onTap: _openChangePasswordDialog,
          ),
          const Padding(
            padding: EdgeInsets.symmetric(vertical: 10),
            child: Divider(color: Color(0xFFEEEEEE), height: 1),
          ),
          _securityOption(
            icon: Icons.mark_email_unread_outlined,
            title: "Reset via email",
            description: "Forgot your password? Send a reset code to your email.",
            onTap: _openResetViaEmailDialog,
          ),
        ],
      ),
    );
  }

  Widget _securityOption({
    required IconData icon,
    required String title,
    required String description,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(12),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 4),
        child: Row(
          children: [
            Container(
              width: 38,
              height: 38,
              decoration: BoxDecoration(
                color: primary.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(10),
              ),
              child: Icon(icon, size: 18, color: primary),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: const TextStyle(
                      fontWeight: FontWeight.w700,
                      fontSize: 14,
                      color: Color(0xFF1C1C1C),
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    description,
                    style: const TextStyle(
                      fontSize: 12,
                      color: Color(0xFF6E6E6E),
                    ),
                  ),
                ],
              ),
            ),
            const Icon(Icons.chevron_right_rounded, color: Color(0xFFBBBBBB)),
          ],
        ),
      ),
    );
  }

  Widget _card({
    required IconData icon,
    required String title,
    required String subtitle,
    required Widget child,
  }) {
    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
        boxShadow: const [
          BoxShadow(
            color: Color(0x0F000000),
            blurRadius: 14,
            offset: Offset(0, 5),
          ),
        ],
      ),
      child: Column(
        children: [
          Row(
            children: [
              CircleAvatar(
                radius: 28,
                backgroundColor: primary.withOpacity(0.12),
                child: Icon(icon, color: primary, size: 30),
              ),
              const SizedBox(width: 14),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: const TextStyle(
                        fontSize: 17,
                        fontWeight: FontWeight.w900,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      subtitle,
                      style: const TextStyle(
                        color: Colors.black54,
                        fontSize: 13,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          child,
        ],
      ),
    );
  }

  Widget _field(
    String label,
    IconData icon,
    TextEditingController controller, {
    required String errorKey,
    TextInputType? keyboardType,
  }) {
    return TextField(
      controller: controller,
      keyboardType: keyboardType,
      decoration: _inputDecoration(label, icon, _errors[errorKey]),
    );
  }

  Widget _dateField() {
    return TextField(
      controller: _birthDate,
      readOnly: true,
      decoration: _inputDecoration(
        "Date of birth",
        Icons.calendar_month,
        _errors["birthDate"],
      ),
      onTap: () async {
        final now = DateTime.now();

        final picked = await showDatePicker(
          context: context,
          initialDate: DateTime(now.year - 18),
          firstDate: DateTime(1900),
          lastDate: now,
        );

        if (picked == null) return;

        setState(() {
          _birthDate.text = _formatDate(picked);
          _errors.remove("birthDate");
        });
      },
    );
  }

  InputDecoration _inputDecoration(String label, IconData icon, String? error) {
    return InputDecoration(
      labelText: label,
      errorText: error,
      prefixIcon: Icon(icon, size: 20),
      filled: true,
      fillColor: const Color(0xFFF8F6FB),
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(14)),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: Color(0xFFE9E1EE)),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: primary, width: 1.6),
      ),
      errorMaxLines: 3,
    );
  }

  String _formatDate(DateTime date) => DateFormat("dd.MM.yyyy").format(date);

  String? _toIsoDate(String value) {
    if (value.trim().isEmpty) return null;
    return DateFormat("dd.MM.yyyy").parse(value).toIso8601String();
  }
}

class _ChangePasswordDialog extends StatefulWidget {
  const _ChangePasswordDialog();

  @override
  State<_ChangePasswordDialog> createState() => _ChangePasswordDialogState();
}

class _ChangePasswordDialogState extends State<_ChangePasswordDialog> {
  static const Color primary = Color(0xFF9C27B0);

  final _currentPassword = TextEditingController();
  final _newPassword = TextEditingController();
  final _confirmPassword = TextEditingController();

  final Map<String, String?> _errors = {};

  bool _submitting = false;
  bool _hideCurrent = true;
  bool _hideNew = true;
  bool _hideConfirm = true;

  @override
  void initState() {
    super.initState();

    for (final c in [_currentPassword, _newPassword, _confirmPassword]) {
      c.addListener(() {
        if (_errors.isNotEmpty) {
          setState(() => _errors.clear());
        }
      });
    }
  }

  @override
  void dispose() {
    _currentPassword.dispose();
    _newPassword.dispose();
    _confirmPassword.dispose();
    super.dispose();
  }

  bool _validate() {
    _errors.clear();

    var ok = ValidationEngine.validate([
      Rules.requiredText(
        "currentPassword",
        _currentPassword.text,
        "Current password is required.",
      ),
      Rules.strongPassword("newPassword", _newPassword.text),
      Rules.strongPassword("confirmPassword", _confirmPassword.text),
    ], (field, message) => _errors[field] = message);

    if (_confirmPassword.text.trim() != _newPassword.text.trim()) {
      ok = false;
      _errors["confirmPassword"] = "Passwords do not match.";
    }

    setState(() {});
    return ok;
  }

  Future<void> _submit() async {
    if (!_validate()) return;

    setState(() => _submitting = true);

    try {
      await context.read<UserProvider>().changePassword({
        "currentPassword": _currentPassword.text.trim(),
        "newPassword": _newPassword.text.trim(),
        "confirmPassword": _confirmPassword.text.trim(),
      });

      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (e) {
      if (!mounted) return;

      setState(() => _submitting = false);

      SnackbarHelper.showError(context, e.toString());
    }
  }

  InputDecoration _decoration({
    required String label,
    required IconData icon,
    required bool hidden,
    required VoidCallback onToggle,
    String? error,
  }) {
    return InputDecoration(
      labelText: label,
      errorText: error,
      prefixIcon: Icon(icon, size: 20),
      suffixIcon: IconButton(
        onPressed: onToggle,
        icon: Icon(hidden ? Icons.visibility_off : Icons.visibility),
      ),
      filled: true,
      fillColor: const Color(0xFFF8F6FB),
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(14)),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: Color(0xFFE9E1EE)),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: primary, width: 1.6),
      ),
      errorMaxLines: 3,
    );
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(22)),
      title: const Row(
        children: [
          Icon(Icons.lock_outline_rounded, color: primary),
          SizedBox(width: 8),
          Expanded(
            child: Text(
              "Change password",
              style: TextStyle(fontWeight: FontWeight.w900),
            ),
          ),
        ],
      ),
      content: SizedBox(
        width: 380,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            _passwordField(
              controller: _currentPassword,
              label: "Current password",
              icon: Icons.lock_clock_outlined,
              hidden: _hideCurrent,
              error: _errors["currentPassword"],
              onToggle: () => setState(() => _hideCurrent = !_hideCurrent),
            ),
            const SizedBox(height: 12),
            _passwordField(
              controller: _newPassword,
              label: "New password",
              icon: Icons.lock_reset_outlined,
              hidden: _hideNew,
              error: _errors["newPassword"],
              onToggle: () => setState(() => _hideNew = !_hideNew),
            ),
            const SizedBox(height: 12),
            _passwordField(
              controller: _confirmPassword,
              label: "Confirm new password",
              icon: Icons.verified_user_outlined,
              hidden: _hideConfirm,
              error: _errors["confirmPassword"],
              onToggle: () => setState(() => _hideConfirm = !_hideConfirm),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: _submitting ? null : () => Navigator.pop(context, false),
          child: const Text("Cancel"),
        ),
        ElevatedButton(
          onPressed: _submitting ? null : _submit,
          style: ElevatedButton.styleFrom(
            backgroundColor: primary,
            foregroundColor: Colors.white,
          ),
          child: _submitting
              ? const SizedBox(
                  width: 18,
                  height: 18,
                  child: CircularProgressIndicator(
                    color: Colors.white,
                    strokeWidth: 2,
                  ),
                )
              : const Text("Change"),
        ),
      ],
    );
  }

  Widget _passwordField({
    required TextEditingController controller,
    required String label,
    required IconData icon,
    required bool hidden,
    required VoidCallback onToggle,
    String? error,
  }) {
    return TextField(
      controller: controller,
      obscureText: hidden,
      decoration: _decoration(
        label: label,
        icon: icon,
        hidden: hidden,
        onToggle: onToggle,
        error: error,
      ),
    );
  }
}

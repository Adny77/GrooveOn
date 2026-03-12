import 'dart:io';

import 'package:flutter/material.dart';
import 'package:grooveon_desktop/config/api_config.dart';
import 'package:grooveon_desktop/dialogs/base_dialogs_frame.dart';
import 'package:grooveon_desktop/dialogs/confirmation_dialogs.dart';
import 'package:grooveon_desktop/helper/image_helper.dart';
import 'package:grooveon_desktop/providers/image_provider.dart';
import 'package:grooveon_desktop/providers/user_provider.dart';
import 'package:grooveon_desktop/routes/app_routes.dart';
import 'package:grooveon_desktop/utils/session.dart';
import 'package:grooveon_desktop/widgets/sidebar_item_widget.dart';
import 'package:provider/provider.dart';

import '../models/user.dart';

class SidebarWidget extends StatefulWidget {
  final int selectedIndex;
  final Function(int) onItemSelected;

  const SidebarWidget({
    super.key,
    required this.selectedIndex,
    required this.onItemSelected,
  });

  static const Color primaryColor = Color(0xFF9C27B0);
  static const Color sidebarColor = Color(0xFFF1F1F3);
  static const Color borderColor = Color(0xFFD9D9DE);
  static const Color textColor = Color(0xFF222222);
  static const Color subTextColor = Color(0xFF6F6F78);

  @override
  State<SidebarWidget> createState() => _SidebarWidgetState();
}

class _SidebarWidgetState extends State<SidebarWidget> {
  late UserProvider _userProvider;

  User? _loggedUser;
  bool _isLoadingUser = true;

  File? _pickedImage;
  bool _isImageChanged = false;

  final _formKey = GlobalKey<FormState>();

  late TextEditingController _firstNameController;
  late TextEditingController _lastNameController;
  late TextEditingController _emailController;
  late TextEditingController _usernameController;
  late TextEditingController _phoneNumberController;
  late TextEditingController _birthDateController;

  @override
  void initState() {
    super.initState();

    _firstNameController = TextEditingController();
    _lastNameController = TextEditingController();
    _emailController = TextEditingController();
    _usernameController = TextEditingController();
    _phoneNumberController = TextEditingController();
    _birthDateController = TextEditingController();

    WidgetsBinding.instance.addPostFrameCallback((_) {
      _userProvider = context.read<UserProvider>();
      _loadLoggedUser();
    });
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _emailController.dispose();
    _usernameController.dispose();
    _phoneNumberController.dispose();
    _birthDateController.dispose();
    super.dispose();
  }

  Future<void> _loadLoggedUser() async {
    try {
      if (Session.userId == null) {
        if (!mounted) return;
        setState(() {
          _loggedUser = null;
          _isLoadingUser = false;
        });
        return;
      }

      if (mounted) {
        setState(() {
          _isLoadingUser = true;
        });
      }

      final user = await _userProvider.getById(Session.userId!);

      if (!mounted) return;
      setState(() {
        _loggedUser = user;
        _isLoadingUser = false;
      });
    } catch (e) {
      if (!mounted) return;
      setState(() {
        _isLoadingUser = false;
      });
    }
  }

  Future<void> _logout() async {
    final confirmed = await ConfirmDialogs.yesNoConfirmation(
      context,
      title: "Odjava",
      question: "Da li ste sigurni da se želite odjaviti?",
      yesText: "Da, odjavi me",
      noText: "Otkaži",
    );

    if (!confirmed) return;

    Session.odjava();

    if (!mounted) return;
    Navigator.of(context).pushNamedAndRemoveUntil(
      AppRoutes.login,
      (route) => false,
    );
  }

  String? _formatDateForApi(String value) {
    if (value.trim().isEmpty) return null;

    try {
      final cleaned = value.replaceAll(' ', '').replaceAll('.', '');
      final parts = cleaned.split(RegExp(r'[-/]'));

      if (parts.length == 3) {
        return DateTime.parse(
          "${parts[2]}-${parts[1].padLeft(2, '0')}-${parts[0].padLeft(2, '0')}",
        ).toIso8601String();
      }

      final dotParts = value
          .replaceAll(' ', '')
          .split('.')
          .where((e) => e.isNotEmpty)
          .toList();

      if (dotParts.length == 3) {
        return DateTime.parse(
          "${dotParts[2]}-${dotParts[1].padLeft(2, '0')}-${dotParts[0].padLeft(2, '0')}",
        ).toIso8601String();
      }

      return null;
    } catch (_) {
      return null;
    }
  }

  Widget _datePickerField({
    required BuildContext context,
    required String label,
    required TextEditingController controller,
    required VoidCallback onAnyChanged,
  }) {
    return TextFormField(
      controller: controller,
      readOnly: true,
      decoration: InputDecoration(
        labelText: label,
        filled: true,
        fillColor: const Color(0xFFF9F9FB),
        suffixIcon: const Icon(Icons.calendar_month_outlined),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: SidebarWidget.borderColor),
        ),
      ),
      validator: (v) =>
          v == null || v.isEmpty ? "Datum rođenja je obavezan." : null,
      onTap: () async {
        final now = DateTime.now();

        final picked = await showDatePicker(
          context: context,
          initialDate: DateTime(now.year - 18),
          firstDate: DateTime(1900),
          lastDate: now,
        );

        if (picked == null) return;

        controller.text =
            "${picked.day.toString().padLeft(2, '0')}."
            "${picked.month.toString().padLeft(2, '0')}."
            "${picked.year}.";

        onAnyChanged();
      },
    );
  }

  Widget _settingsField({
    required String label,
    required TextEditingController controller,
    TextInputType? keyboardType,
    String? Function(String?)? validator,
    void Function(String)? onChanged,
  }) {
    return TextFormField(
      controller: controller,
      keyboardType: keyboardType,
      validator: validator,
      onChanged: onChanged,
      decoration: InputDecoration(
        labelText: label,
        filled: true,
        fillColor: const Color(0xFFF9F9FB),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: SidebarWidget.borderColor),
        ),
      ),
    );
  }

  Future<void> _showUserSettingsDialog() async {
    await _loadLoggedUser();
    if (!mounted) return;

    _pickedImage = null;
    _isImageChanged = false;

    final user = _loggedUser;
    if (user != null) {
      _firstNameController.text = user.firstName ?? '';
      _lastNameController.text = user.lastName ?? '';
      _emailController.text = user.email!;
      _usernameController.text = user.username!;
      _phoneNumberController.text = user.phoneNumber ?? '';

      if (user.dateOfBirth != null) {
        _birthDateController.text =
            "${user.dateOfBirth!.day.toString().padLeft(2, '0')}."
            "${user.dateOfBirth!.month.toString().padLeft(2, '0')}."
            "${user.dateOfBirth!.year}.";
      } else {
        _birthDateController.text = '';
      }
    }

    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => StatefulBuilder(
        builder: (context, setStateDialog) {
          bool hasChanges() {
            if (_isImageChanged) return true;
            if (user == null) return false;

            final originalBirthDate = user.dateOfBirth != null
                ? "${user.dateOfBirth!.day.toString().padLeft(2, '0')}."
                  "${user.dateOfBirth!.month.toString().padLeft(2, '0')}."
                  "${user.dateOfBirth!.year}."
                : "";

            return _firstNameController.text != (user.firstName ?? '') ||
                _lastNameController.text != (user.lastName ?? '') ||
                _emailController.text != user.email ||
                _usernameController.text != user.username ||
                _phoneNumberController.text != (user.phoneNumber ?? '') ||
                _birthDateController.text != originalBirthDate;
          }

          return BaseDialog(
            title: "Profile settings",
            width: 680,
            height: 690,
            onClose: () {
              Navigator.pop(context);
            },
            child: _userSettingsContent(
              user: user,
              onChangeImage: () async {
                final picked = await ImageHelper.openImagePicker();
                if (picked == null) return;
                setStateDialog(() {
                  _pickedImage = picked;
                  _isImageChanged = true;
                });
              },
              onAnyChanged: () => setStateDialog(() {}),
              isSaveEnabled: hasChanges(),
              onSave: () async {
                final ok = _formKey.currentState?.validate() ?? false;
                if (!ok || user == null) return;

                String? finalImage = user.userImage;
                if (_pickedImage != null) {
                  final uploadedFileName = await ImageAppProvider.upload(
                    file: _pickedImage!,
                    folder: "users",
                  );
                  finalImage = uploadedFileName;
                }

                await _userProvider.update(Session.userId!, {
                  'firstName': _firstNameController.text.trim(),
                  'lastName': _lastNameController.text.trim(),
                  'email': _emailController.text.trim(),
                  'username': _usernameController.text.trim(),
                  'phoneNumber': _phoneNumberController.text.trim(),
                  'dateOfBirth': _formatDateForApi(_birthDateController.text),
                  'userImage': finalImage,
                });

                await _loadLoggedUser();
                if (!context.mounted) return;
                Navigator.pop(context);
              },
            ),
          );
        },
      ),
    );
  }

  Widget _userSettingsContent({
    required User? user,
    required Future<void> Function() onChangeImage,
    required VoidCallback onAnyChanged,
    required bool isSaveEnabled,
    required Future<void> Function() onSave,
  }) {
    if (user == null) {
      return const Center(
        child: Text("Korisnik nije učitan."),
      );
    }

    final fullName =
        "${user.firstName ?? ''} ${user.lastName ?? ''}".trim().isEmpty
            ? user.username
            : "${user.firstName ?? ''} ${user.lastName ?? ''}".trim();

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: const Color(0xFFF9F9FB),
            borderRadius: BorderRadius.circular(16),
            border: Border.all(color: SidebarWidget.borderColor),
          ),
          child: Row(
            children: [
              ClipRRect(
                borderRadius: BorderRadius.circular(18),
                child: Container(
                  width: 130,
                  height: 130,
                  color: const Color(0x11000000),
                  child: _pickedImage != null
                      ? Image.file(_pickedImage!, fit: BoxFit.cover)
                      : (ImageHelper.hasValidImage(user.userImage)
                          ? Image.network(
                              ImageHelper.isHttp(user.userImage!)
                                  ? user.userImage!
                                  : "${ApiConfig.apiBase}/images/users/${user.userImage!}",
                              fit: BoxFit.cover,
                              errorBuilder: (_, __, ___) =>
                                  ImageHelper.userPlaceholder(user.username!),
                            )
                          : ImageHelper.userPlaceholder(user.username!)),
                ),
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      fullName!,
                      style: const TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.w800,
                        color: SidebarWidget.textColor,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      user.email!,
                      style: const TextStyle(
                        fontSize: 14,
                        color: SidebarWidget.subTextColor,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                    const SizedBox(height: 12),
                    OutlinedButton.icon(
                      onPressed: () async {
                        await onChangeImage();
                        onAnyChanged();
                      },
                      icon: const Icon(Icons.photo_camera_outlined, size: 18),
                      label: const Text("Change image"),
                      style: OutlinedButton.styleFrom(
                        foregroundColor: SidebarWidget.primaryColor,
                        side: const BorderSide(
                          color: SidebarWidget.primaryColor,
                        ),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),
        Expanded(
          child: Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(16),
              border: Border.all(color: SidebarWidget.borderColor),
            ),
            child: Form(
              key: _formKey,
              child: SingleChildScrollView(
                child: Column(
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: _settingsField(
                            label: "First name",
                            controller: _firstNameController,
                            validator: (v) => (v == null || v.trim().isEmpty)
                                ? "First name is required."
                                : null,
                            onChanged: (_) => onAnyChanged(),
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _settingsField(
                            label: "Last name",
                            controller: _lastNameController,
                            validator: (v) => (v == null || v.trim().isEmpty)
                                ? "Last name is required."
                                : null,
                            onChanged: (_) => onAnyChanged(),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        Expanded(
                          child: _settingsField(
                            label: "Email",
                            controller: _emailController,
                            keyboardType: TextInputType.emailAddress,
                            validator: (v) => (v == null || !v.contains("@"))
                                ? "Enter a valid email."
                                : null,
                            onChanged: (_) => onAnyChanged(),
                          ),
                        ),
                        const SizedBox(width: 12),
                        Expanded(
                          child: _settingsField(
                            label: "Username",
                            controller: _usernameController,
                            validator: (v) => (v == null || v.trim().length < 3)
                                ? "Username min 3 characters."
                                : null,
                            onChanged: (_) => onAnyChanged(),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    _datePickerField(
                      context: context,
                      label: "Date of birth",
                      controller: _birthDateController,
                      onAnyChanged: onAnyChanged,
                    ),
                    const SizedBox(height: 12),
                    _settingsField(
                      label: "Phone number",
                      controller: _phoneNumberController,
                      keyboardType: TextInputType.phone,
                      validator: (v) => (v == null || v.trim().isEmpty)
                          ? "Phone number is required."
                          : null,
                      onChanged: (_) => onAnyChanged(),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
        const SizedBox(height: 16),
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            OutlinedButton.icon(
              onPressed: _logout,
              icon: const Icon(Icons.logout, color: Colors.red),
              label: const Text(
                "Log out",
                style: TextStyle(
                  color: Colors.red,
                  fontWeight: FontWeight.w700,
                ),
              ),
              style: OutlinedButton.styleFrom(
                side: const BorderSide(color: Colors.red),
                padding: const EdgeInsets.symmetric(
                  horizontal: 16,
                  vertical: 12,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
            ),
            ElevatedButton(
              onPressed: isSaveEnabled ? () => onSave() : null,
              style: ElevatedButton.styleFrom(
                backgroundColor: SidebarWidget.primaryColor,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(
                  horizontal: 18,
                  vertical: 12,
                ),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
              ),
              child: const Text(
                "Save changes",
                style: TextStyle(fontWeight: FontWeight.w800),
              ),
            ),
          ],
        ),
      ],
    );
  }

  String _displayName() {
    if (_isLoadingUser) return "Loading...";
    if (_loggedUser == null) return "Jane Doe";

    final fullName =
        "${_loggedUser?.firstName ?? ''} ${_loggedUser?.lastName ?? ''}".trim();

    if (fullName.isNotEmpty) return fullName;
    return _loggedUser?.username ?? "Jane Doe";
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      width: 150,
      decoration: const BoxDecoration(
        color: SidebarWidget.sidebarColor,
        border: Border(
          right: BorderSide(color: SidebarWidget.borderColor),
        ),
      ),
      child: Column(
        children: [
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 18, horizontal: 12),
            decoration: const BoxDecoration(
              border: Border(
                bottom: BorderSide(color: SidebarWidget.borderColor),
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Text(
                  "Welcome",
                  style: TextStyle(
                    fontSize: 11,
                    color: SidebarWidget.subTextColor,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  _displayName(),
                  style: const TextStyle(
                    fontSize: 15,
                    fontWeight: FontWeight.w700,
                    color: SidebarWidget.textColor,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 10),
          SidebarItemWidget(
            icon: Icons.library_music_outlined,
            title: "Music",
            selected: widget.selectedIndex == 0,
            onTap: () => widget.onItemSelected(0),
          ),
          SidebarItemWidget(
            icon: Icons.person_outline,
            title: "Users",
            selected: widget.selectedIndex == 1,
            onTap: () => widget.onItemSelected(1),
          ),
          SidebarItemWidget(
            icon: Icons.payments_outlined,
            title: "Income",
            selected: widget.selectedIndex == 2,
            onTap: () => widget.onItemSelected(2),
          ),
          const Spacer(),
          SidebarItemWidget(
            icon: Icons.settings_outlined,
            title: "Settings",
            selected: false,
            onTap: () => _showUserSettingsDialog(),
          ),
          SidebarItemWidget(
            icon: Icons.logout,
            title: "Log out",
            selected: false,
            onTap: () => _logout(),
          ),
          const SizedBox(height: 12),
        ],
      ),
    );
  }
}
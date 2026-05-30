import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/exception_read_helper.dart';
import 'package:grooveon_mobile/providers/user_provider.dart';
import 'package:grooveon_mobile/validation/validation_model/validation_rules.dart';
import 'package:grooveon_mobile/validation/validation_use/universal_error_removal.dart';
import 'package:grooveon_mobile/validation/validation_use/universal_validator.dart';
import 'package:provider/provider.dart';

class ForgotPasswordDialog extends StatefulWidget {
  const ForgotPasswordDialog({super.key});

  @override
  State<ForgotPasswordDialog> createState() => _ForgotPasswordDialogState();
}

class _ForgotPasswordDialogState extends State<ForgotPasswordDialog> {
  static const Color primaryPurple = Color(0xFF9C27B0);
  static const Color darkPurple = Color(0xFF4A148C);
  static const Color subText = Color(0xFF6E6E6E);
  static const Color textDark = Color(0xFF1C1C1C);

  int _step = 1;
  bool _isLoading = false;

  final _emailController = TextEditingController();
  final Map<String, String?> _step1Errors = {};

  final _tokenController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();
  final Map<String, String?> _step2Errors = {};
  bool _obscurePassword = true;
  bool _obscureConfirm = true;

  @override
  void initState() {
    super.initState();
    ErrorAutoRemoval.removeErrorOnTextField(
      field: 'email',
      fieldErrors: _step1Errors,
      controller: _emailController,
      setState: () => setState(() {}),
    );
    ErrorAutoRemoval.removeErrorOnTextField(
      field: 'token',
      fieldErrors: _step2Errors,
      controller: _tokenController,
      setState: () => setState(() {}),
    );
    ErrorAutoRemoval.removeErrorOnTextField(
      field: 'password',
      fieldErrors: _step2Errors,
      controller: _passwordController,
      setState: () => setState(() {}),
    );
    ErrorAutoRemoval.removeErrorOnTextField(
      field: 'confirm',
      fieldErrors: _step2Errors,
      controller: _confirmController,
      setState: () => setState(() {}),
    );
  }

  @override
  void dispose() {
    _emailController.dispose();
    _tokenController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  Future<void> _sendResetCode() async {
    setState(() => _step1Errors.clear());

    final isValid = ValidationEngine.validate(
      [Rules.email('email', _emailController.text)],
      (field, message) => setState(() => _step1Errors[field] = message),
    );

    if (!isValid) return;

    setState(() => _isLoading = true);

    try {
      await context.read<UserProvider>().forgotPassword(_emailController.text.trim());
      if (!mounted) return;
      setState(() => _step = 2);
    } catch (e) {
      if (!mounted) return;
      setState(() => _step1Errors['email'] = extractErrorMessage(e));
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _resetPassword() async {
    setState(() => _step2Errors.clear());

    final password = _passwordController.text;
    final confirm = _confirmController.text;

    final isValid = ValidationEngine.validate(
      [
        Rules.requiredText('token', _tokenController.text, 'Reset token is required.'),
        Rules.strongPassword('password', password),
      ],
      (field, message) => setState(() => _step2Errors[field] = message),
    );

    if (password != confirm) {
      setState(() => _step2Errors['confirm'] = 'Passwords do not match.');
    }

    if (!isValid || password != confirm) return;

    setState(() => _isLoading = true);

    try {
      await context.read<UserProvider>().resetPassword(
            _tokenController.text.trim(),
            password,
          );
      if (!mounted) return;
      Navigator.of(context).pop();
    } catch (e) {
      if (!mounted) return;
      setState(() => _step2Errors['token'] = extractErrorMessage(e));
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  InputDecoration _inputDecoration({
    required String hint,
    required IconData icon,
    Widget? suffix,
    String? errorText,
  }) {
    return InputDecoration(
      hintText: hint,
      hintStyle: const TextStyle(
        color: Color(0xFF8A8A8A),
        fontWeight: FontWeight.w500,
      ),
      filled: true,
      fillColor: const Color(0xFFF8F6FB),
      prefixIcon: Icon(icon, color: primaryPurple),
      suffixIcon: suffix,
      errorText: errorText,
      contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 18),
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(18),
        borderSide: BorderSide.none,
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(18),
        borderSide: const BorderSide(color: Color(0xFFE7DDF0), width: 1.1),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(18),
        borderSide: const BorderSide(color: darkPurple, width: 2),
      ),
      errorBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(18),
        borderSide: const BorderSide(color: Colors.redAccent, width: 1.5),
      ),
      focusedErrorBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(18),
        borderSide: const BorderSide(color: Colors.redAccent, width: 2),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      backgroundColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(28)),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 460),
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(28),
            border: Border.all(
              color: Colors.white.withValues(alpha: 0.55),
              width: 1.2,
            ),
            boxShadow: const [
              BoxShadow(
                color: Color(0x33000000),
                blurRadius: 30,
                offset: Offset(0, 16),
              ),
            ],
          ),
          child: SingleChildScrollView(
            padding: const EdgeInsets.fromLTRB(22, 24, 22, 20),
            child: _step == 1 ? _buildStep1() : _buildStep2(),
          ),
        ),
      ),
    );
  }

  Widget _buildStep1() {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _buildHeader(
          icon: Icons.lock_reset_rounded,
          title: 'Forgot Password',
          subtitle:
              'Enter the email linked to your account and we\'ll send you a reset code.',
        ),
        const SizedBox(height: 20),
        TextField(
          controller: _emailController,
          enabled: !_isLoading,
          keyboardType: TextInputType.emailAddress,
          style: const TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.w700,
            color: textDark,
          ),
          decoration: _inputDecoration(
            hint: 'Your email address',
            icon: Icons.email_rounded,
            errorText: _step1Errors['email'],
          ),
          onSubmitted: (_) {
            if (!_isLoading) _sendResetCode();
          },
        ),
        const SizedBox(height: 20),
        _buildActions(
          onCancel: () => Navigator.of(context).pop(),
          cancelLabel: 'Cancel',
          onConfirm: _sendResetCode,
          confirmLabel: 'Send Reset Code',
        ),
      ],
    );
  }

  Widget _buildStep2() {
    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _buildHeader(
          icon: Icons.vpn_key_rounded,
          title: 'Reset Password',
          subtitle: 'Paste the reset code from your email and set a new password.',
        ),
        const SizedBox(height: 12),
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
          decoration: BoxDecoration(
            color: const Color(0xFFF3E5F5),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Row(
            children: [
              const Icon(Icons.info_outline_rounded, size: 16, color: primaryPurple),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  'Reset code sent to ${_emailController.text.trim()}',
                  style: const TextStyle(
                    fontSize: 12,
                    color: darkPurple,
                    fontWeight: FontWeight.w500,
                  ),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 16),
        TextField(
          controller: _tokenController,
          enabled: !_isLoading,
          style: const TextStyle(
            fontSize: 15,
            fontWeight: FontWeight.w600,
            color: textDark,
          ),
          decoration: _inputDecoration(
            hint: 'Paste reset code',
            icon: Icons.key_rounded,
            errorText: _step2Errors['token'],
          ),
        ),
        const SizedBox(height: 14),
        TextField(
          controller: _passwordController,
          enabled: !_isLoading,
          obscureText: _obscurePassword,
          keyboardType: TextInputType.visiblePassword,
          style: const TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.w700,
            color: textDark,
          ),
          decoration: _inputDecoration(
            hint: 'New password',
            icon: Icons.lock_rounded,
            errorText: _step2Errors['password'],
            suffix: IconButton(
              onPressed: () =>
                  setState(() => _obscurePassword = !_obscurePassword),
              icon: Icon(
                _obscurePassword
                    ? Icons.visibility_off_rounded
                    : Icons.visibility_rounded,
                color: subText,
              ),
            ),
          ),
        ),
        const SizedBox(height: 14),
        TextField(
          controller: _confirmController,
          enabled: !_isLoading,
          obscureText: _obscureConfirm,
          keyboardType: TextInputType.visiblePassword,
          style: const TextStyle(
            fontSize: 16,
            fontWeight: FontWeight.w700,
            color: textDark,
          ),
          decoration: _inputDecoration(
            hint: 'Confirm new password',
            icon: Icons.lock_outline_rounded,
            errorText: _step2Errors['confirm'],
            suffix: IconButton(
              onPressed: () =>
                  setState(() => _obscureConfirm = !_obscureConfirm),
              icon: Icon(
                _obscureConfirm
                    ? Icons.visibility_off_rounded
                    : Icons.visibility_rounded,
                color: subText,
              ),
            ),
          ),
        ),
        const SizedBox(height: 20),
        _buildActions(
          onCancel: () => setState(() {
            _step = 1;
            _step2Errors.clear();
          }),
          cancelLabel: 'Back',
          onConfirm: _resetPassword,
          confirmLabel: 'Reset Password',
        ),
      ],
    );
  }

  Widget _buildHeader({
    required IconData icon,
    required String title,
    required String subtitle,
  }) {
    return Column(
      children: [
        Container(
          width: 64,
          height: 64,
          decoration: const BoxDecoration(
            shape: BoxShape.circle,
            gradient: LinearGradient(
              colors: [primaryPurple, darkPurple],
            ),
          ),
          child: Icon(icon, color: Colors.white, size: 30),
        ),
        const SizedBox(height: 14),
        Text(
          title,
          textAlign: TextAlign.center,
          style: const TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.w900,
            letterSpacing: 0.5,
            color: darkPurple,
          ),
        ),
        const SizedBox(height: 8),
        Text(
          subtitle,
          textAlign: TextAlign.center,
          style: const TextStyle(fontSize: 13, color: subText, height: 1.5),
        ),
      ],
    );
  }

  Widget _buildActions({
    required VoidCallback onCancel,
    required String cancelLabel,
    required VoidCallback onConfirm,
    required String confirmLabel,
  }) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.end,
      children: [
        TextButton(
          onPressed: _isLoading ? null : onCancel,
          child: Text(
            cancelLabel,
            style: const TextStyle(
              color: subText,
              fontWeight: FontWeight.w600,
            ),
          ),
        ),
        const SizedBox(width: 8),
        SizedBox(
          height: 48,
          child: ElevatedButton(
            onPressed: _isLoading ? null : onConfirm,
            style: ElevatedButton.styleFrom(
              elevation: 0,
              backgroundColor: darkPurple,
              foregroundColor: Colors.white,
              disabledBackgroundColor: darkPurple.withValues(alpha: 0.65),
              padding: const EdgeInsets.symmetric(horizontal: 20),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(18),
              ),
            ),
            child: _isLoading
                ? const SizedBox(
                    width: 18,
                    height: 18,
                    child: CircularProgressIndicator(
                      strokeWidth: 2,
                      color: Colors.white,
                    ),
                  )
                : Text(
                    confirmLabel,
                    style: const TextStyle(fontWeight: FontWeight.w700),
                  ),
          ),
        ),
      ],
    );
  }
}

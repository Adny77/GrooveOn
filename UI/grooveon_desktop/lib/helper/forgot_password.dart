import 'package:flutter/material.dart';
import 'package:grooveon_desktop/providers/user_provider.dart';
import 'package:grooveon_desktop/validation/user_validation.dart';
import 'package:provider/provider.dart';

class ForgotPasswordDialog extends StatefulWidget {
  const ForgotPasswordDialog({super.key});

  @override
  State<ForgotPasswordDialog> createState() => _ForgotPasswordDialogState();
}

class _ForgotPasswordDialogState extends State<ForgotPasswordDialog> {
  static const Color _primary = Color(0xFF9C27B0);
  static const Color _primaryDark = Color(0xFF4A148C);
  static const Color _softPurple = Color(0xFFF3E5F5);
  static const Color _muted = Color(0xFF6B7280);
  static const Color _border = Color(0xFFE6D8EE);
  static const double _radius = 18;

  // Step 1: email
  final TextEditingController _emailController = TextEditingController();
  // Step 2: token + new password
  final TextEditingController _tokenController = TextEditingController();
  final TextEditingController _newPasswordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();

  bool _step2 = false;
  bool _obscureNew = true;
  bool _obscureConfirm = true;
  String? _error;
  String? _success;
  bool _isLoading = false;

  @override
  void dispose() {
    _emailController.dispose();
    _tokenController.dispose();
    _newPasswordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  Future<void> _submitStep1() async {
    setState(() {
      _error = null;
      _success = null;
    });

    final email = _emailController.text.trim();
    final err = Validators.email(email);
    if (err != null) {
      setState(() => _error = err);
      return;
    }

    setState(() => _isLoading = true);

    try {
      await context.read<UserProvider>().forgotPassword(email);
      if (!mounted) return;
      setState(() {
        _step2 = true;
        _success =
            "If the email exists, a reset code has been sent. Check your inbox.";
      });
    } catch (_) {
      if (!mounted) return;
      setState(() => _error = "Failed to send email. Please try again.");
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _submitStep2() async {
    setState(() {
      _error = null;
      _success = null;
    });

    final token = _tokenController.text.trim();
    final newPass = _newPasswordController.text;
    final confirmPass = _confirmPasswordController.text;

    if (token.isEmpty) {
      setState(() => _error = "Reset code is required.");
      return;
    }
    if (newPass.length < 8) {
      setState(() => _error = "Password must be at least 8 characters.");
      return;
    }
    if (newPass != confirmPass) {
      setState(() => _error = "Passwords do not match.");
      return;
    }

    setState(() => _isLoading = true);

    try {
      await context.read<UserProvider>().resetPassword(token, newPass);
      if (!mounted) return;
      Navigator.of(context).pop(true);
    } catch (e) {
      if (!mounted) return;
      setState(() => _error = e.toString().replaceFirst("Exception: ", ""));
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  InputDecoration _inputDeco(String hint, IconData icon, {Widget? suffix}) {
    return InputDecoration(
      hintText: hint,
      prefixIcon: Icon(icon, size: 20, color: _primary),
      suffixIcon: suffix,
      filled: true,
      fillColor: _softPurple.withValues(alpha: 0.45),
      isDense: true,
      contentPadding:
          const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
      border: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: _border),
      ),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: _border),
      ),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: _primaryDark, width: 1.8),
      ),
    );
  }

  Widget _messageBox({required String text, required bool isError}) {
    final bg = isError ? const Color(0xFFFFEEF8) : const Color(0xFFF3E5F5);
    final fg = isError ? const Color(0xFFC2185B) : _primaryDark;
    final icon = isError
        ? Icons.error_outline_rounded
        : Icons.check_circle_outline_rounded;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
      decoration: BoxDecoration(
        color: bg,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: fg.withValues(alpha: 0.25)),
      ),
      child: Row(
        children: [
          Icon(icon, color: fg, size: 18),
          const SizedBox(width: 10),
          Expanded(
            child: Text(
              text,
              style: TextStyle(
                color: fg,
                fontWeight: FontWeight.w800,
                fontSize: 12.8,
                height: 1.3,
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildStep1Body() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          "Enter the email connected to your GrooveOn account and we will send you a reset code.",
          style: TextStyle(
            fontSize: 13.2,
            fontWeight: FontWeight.w700,
            color: _muted,
            height: 1.35,
          ),
        ),
        const SizedBox(height: 14),
        TextField(
          controller: _emailController,
          keyboardType: TextInputType.emailAddress,
          onSubmitted: (_) => _isLoading ? null : _submitStep1(),
          decoration:
              _inputDeco("Email address", Icons.email_outlined),
        ),
        if (_error != null) ...[
          const SizedBox(height: 12),
          _messageBox(text: _error!, isError: true),
        ],
        if (_success != null) ...[
          const SizedBox(height: 12),
          _messageBox(text: _success!, isError: false),
        ],
        const SizedBox(height: 16),
        Row(
          children: [
            OutlinedButton(
              onPressed:
                  _isLoading ? null : () => Navigator.of(context).pop(),
              style: OutlinedButton.styleFrom(
                foregroundColor: _muted,
                side: BorderSide(color: _muted.withValues(alpha: 0.35)),
                padding: const EdgeInsets.symmetric(
                    horizontal: 16, vertical: 12),
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12)),
              ),
              child: const Text("Close",
                  style: TextStyle(fontWeight: FontWeight.w900)),
            ),
            const Spacer(),
            ElevatedButton.icon(
              onPressed: _isLoading ? null : _submitStep1,
              style: ElevatedButton.styleFrom(
                backgroundColor: _primaryDark,
                foregroundColor: Colors.white,
                elevation: 0,
                padding: const EdgeInsets.symmetric(
                    horizontal: 18, vertical: 12),
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12)),
              ),
              icon: _isLoading
                  ? const SizedBox(
                      width: 16,
                      height: 16,
                      child: CircularProgressIndicator(
                          strokeWidth: 2, color: Colors.white),
                    )
                  : const Icon(Icons.send_rounded, size: 18),
              label: Text(
                _isLoading ? "Sending..." : "Send Code",
                style: const TextStyle(fontWeight: FontWeight.w900),
              ),
            ),
          ],
        ),
      ],
    );
  }

  Widget _buildStep2Body() {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const Text(
          "Enter the reset code from your email and choose a new password.",
          style: TextStyle(
            fontSize: 13.2,
            fontWeight: FontWeight.w700,
            color: _muted,
            height: 1.35,
          ),
        ),
        const SizedBox(height: 14),
        TextField(
          controller: _tokenController,
          decoration: _inputDeco("Reset code", Icons.vpn_key_rounded),
        ),
        const SizedBox(height: 10),
        TextField(
          controller: _newPasswordController,
          obscureText: _obscureNew,
          decoration: _inputDeco(
            "New password",
            Icons.lock_outline_rounded,
            suffix: IconButton(
              icon: Icon(
                _obscureNew
                    ? Icons.visibility_off_outlined
                    : Icons.visibility_outlined,
                size: 18,
                color: _muted,
              ),
              onPressed: () => setState(() => _obscureNew = !_obscureNew),
            ),
          ),
        ),
        const SizedBox(height: 10),
        TextField(
          controller: _confirmPasswordController,
          obscureText: _obscureConfirm,
          onSubmitted: (_) => _isLoading ? null : _submitStep2(),
          decoration: _inputDeco(
            "Confirm new password",
            Icons.lock_outline_rounded,
            suffix: IconButton(
              icon: Icon(
                _obscureConfirm
                    ? Icons.visibility_off_outlined
                    : Icons.visibility_outlined,
                size: 18,
                color: _muted,
              ),
              onPressed: () =>
                  setState(() => _obscureConfirm = !_obscureConfirm),
            ),
          ),
        ),
        if (_error != null) ...[
          const SizedBox(height: 12),
          _messageBox(text: _error!, isError: true),
        ],
        const SizedBox(height: 16),
        Row(
          children: [
            OutlinedButton(
              onPressed:
                  _isLoading ? null : () => Navigator.of(context).pop(),
              style: OutlinedButton.styleFrom(
                foregroundColor: _muted,
                side: BorderSide(color: _muted.withValues(alpha: 0.35)),
                padding: const EdgeInsets.symmetric(
                    horizontal: 16, vertical: 12),
                shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12)),
              ),
              child: const Text("Close",
                  style: TextStyle(fontWeight: FontWeight.w900)),
            ),
            const Spacer(),
            ElevatedButton.icon(
                onPressed: _isLoading ? null : _submitStep2,
                style: ElevatedButton.styleFrom(
                  backgroundColor: _primaryDark,
                  foregroundColor: Colors.white,
                  elevation: 0,
                  padding: const EdgeInsets.symmetric(
                      horizontal: 18, vertical: 12),
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12)),
                ),
                icon: _isLoading
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.lock_reset_rounded, size: 18),
                label: Text(
                  _isLoading ? "Resetting..." : "Reset Password",
                  style: const TextStyle(fontWeight: FontWeight.w900),
                ),
              ),
          ],
        ),
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(_radius),
      ),
      child: ConstrainedBox(
        constraints: const BoxConstraints(maxWidth: 480),
        child: Container(
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(_radius),
            border: Border.all(color: _border),
            boxShadow: [
              BoxShadow(
                color: _primary.withValues(alpha: 0.14),
                blurRadius: 22,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(
                    horizontal: 18, vertical: 16),
                decoration: const BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.centerLeft,
                    end: Alignment.centerRight,
                    colors: [_primaryDark, _primary],
                  ),
                  borderRadius: BorderRadius.vertical(
                    top: Radius.circular(_radius),
                  ),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.lock_reset_rounded,
                        color: Colors.white, size: 20),
                    const SizedBox(width: 10),
                    Text(
                      _step2 ? "Reset Password" : "Forgot Password",
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 16.5,
                        fontWeight: FontWeight.w900,
                      ),
                    ),
                    if (_step2) ...[
                      const Spacer(),
                      Container(
                        padding: const EdgeInsets.symmetric(
                            horizontal: 8, vertical: 3),
                        decoration: BoxDecoration(
                          color: Colors.white.withValues(alpha: 0.2),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: const Text(
                          "Step 2 / 2",
                          style: TextStyle(
                              color: Colors.white,
                              fontSize: 11,
                              fontWeight: FontWeight.w700),
                        ),
                      ),
                    ],
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(18, 18, 18, 16),
                child:
                    _step2 ? _buildStep2Body() : _buildStep1Body(),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

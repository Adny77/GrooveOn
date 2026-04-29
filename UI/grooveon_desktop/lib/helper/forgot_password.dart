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

  final TextEditingController _emailController = TextEditingController();

  String? error;
  String? success;
  bool isLoading = false;

  Future<void> _submit() async {
    setState(() {
      error = null;
      success = null;
    });

    final email = _emailController.text.trim();

    final err = Validators.email(email);
    if (err != null) {
      setState(() => error = err);
      return;
    }

    setState(() => isLoading = true);

    try {
      final userProvider = context.read<UserProvider>();

      await userProvider.forgotPassword(email);

      if (!mounted) return;

      setState(() {
        success =
            "Email has been sent. Please check your inbox or spam folder.";
      });
    } catch (e) {
      if (!mounted) return;

      setState(() {
        error = "Failed to send email. Please check your email and try again.";
      });
    } finally {
      if (mounted) {
        setState(() => isLoading = false);
      }
    }
  }

  InputDecoration _inputDecoration() {
    return InputDecoration(
      hintText: "Email address",
      prefixIcon: const Icon(Icons.email_outlined, size: 20, color: _primary),
      filled: true,
      fillColor: _softPurple.withOpacity(0.45),
      isDense: true,
      contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 14),
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
        border: Border.all(color: fg.withOpacity(0.25)),
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

  @override
  void dispose() {
    _emailController.dispose();
    super.dispose();
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
                color: _primary.withOpacity(0.14),
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
                  horizontal: 18,
                  vertical: 16,
                ),
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
                child: const Row(
                  children: [
                    Icon(
                      Icons.lock_reset_rounded,
                      color: Colors.white,
                      size: 20,
                    ),
                    SizedBox(width: 10),
                    Text(
                      "Forgot Password",
                      style: TextStyle(
                        color: Colors.white,
                        fontSize: 16.5,
                        fontWeight: FontWeight.w900,
                      ),
                    ),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.fromLTRB(18, 18, 18, 16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      "Enter the email connected to your GrooveOn account and we will send you password reset instructions.",
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
                      onSubmitted: (_) => isLoading ? null : _submit(),
                      decoration: _inputDecoration(),
                    ),
                    if (error != null) ...[
                      const SizedBox(height: 12),
                      _messageBox(text: error!, isError: true),
                    ],
                    if (success != null) ...[
                      const SizedBox(height: 12),
                      _messageBox(text: success!, isError: false),
                    ],
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        OutlinedButton(
                          onPressed: isLoading
                              ? null
                              : () => Navigator.of(context).pop(),
                          style: OutlinedButton.styleFrom(
                            foregroundColor: _muted,
                            side: BorderSide(color: _muted.withOpacity(0.35)),
                            padding: const EdgeInsets.symmetric(
                              horizontal: 16,
                              vertical: 12,
                            ),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                          child: const Text(
                            "Close",
                            style: TextStyle(fontWeight: FontWeight.w900),
                          ),
                        ),
                        const Spacer(),
                        ElevatedButton.icon(
                          onPressed: isLoading ? null : _submit,
                          style: ElevatedButton.styleFrom(
                            backgroundColor: _primaryDark,
                            foregroundColor: Colors.white,
                            elevation: 0,
                            padding: const EdgeInsets.symmetric(
                              horizontal: 18,
                              vertical: 12,
                            ),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(12),
                            ),
                          ),
                          icon: isLoading
                              ? const SizedBox(
                                  width: 16,
                                  height: 16,
                                  child: CircularProgressIndicator(
                                    strokeWidth: 2,
                                    color: Colors.white,
                                  ),
                                )
                              : const Icon(Icons.send_rounded, size: 18),
                          label: Text(
                            isLoading ? "Sending..." : "Send Email",
                            style: const TextStyle(fontWeight: FontWeight.w900),
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

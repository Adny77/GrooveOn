import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/models/login_request.dart';
import 'package:grooveon_mobile/providers/auth_provider.dart';
import 'package:provider/provider.dart';
import 'package:grooveon_mobile/routes/app_routes.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  static const Color primaryPurple = Color(0xFF9C27B0);
  static const Color darkPurple = Color(0xFF4A148C);
  static const Color lightPurple = Color(0xFFF3E5F5);
  static const Color softPurple = Color(0xFFE1BEE7);
  static const Color textDark = Color(0xFF1C1C1C);
  static const Color subText = Color(0xFF6E6E6E);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  late AuthProvider _authProvider;

  final _formKey = GlobalKey<FormState>();
  final _username = TextEditingController();
  final _password = TextEditingController();

  bool _isLoading = false;
  bool _obscure = true;

  @override
  void initState() {
    super.initState();
    _authProvider = context.read<AuthProvider>();
  }

  @override
  void dispose() {
    _username.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    FocusScope.of(context).unfocus();

    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    try {
      final result = await _authProvider.prijava(
        LoginRequest(
          username: _username.text.trim(),
          password: _password.text,
        ),
      );

      if (!mounted) return;

      if (result == "ZABRANJENO") {
        SnackbarHelper.showError(
          context,
          'Nemate privilegije za pristup mobilnoj aplikaciji.',
        );
        return;
      }

      if (result != "OK") {
        SnackbarHelper.showError(
          context,
          'Pogrešno korisničko ime ili lozinka.',
        );
        return;
      }

      Navigator.pushReplacementNamed(context, AppRoutes.navigation);
    } catch (e) {
      if (!mounted) return;
      SnackbarHelper.showError(context, '$e');
    } finally {
      if (mounted) {
        setState(() => _isLoading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = MediaQuery.of(context).viewInsets.bottom;

    return Scaffold(
      body: Container(
        width: double.infinity,
        decoration: const BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topLeft,
            end: Alignment.bottomRight,
            colors: [
              LoginScreen.lightPurple,
              Color(0xFFD1A3FF),
              LoginScreen.primaryPurple,
              LoginScreen.darkPurple,
            ],
            stops: [0.0, 0.28, 0.65, 1.0],
          ),
        ),
        child: Stack(
          children: [
            Positioned(
              top: -70,
              right: -40,
              child: Container(
                width: 220,
                height: 220,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: Colors.white.withOpacity(0.10),
                ),
              ),
            ),
            Positioned(
              bottom: -90,
              left: -60,
              child: Container(
                width: 260,
                height: 260,
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  color: Colors.white.withOpacity(0.08),
                ),
              ),
            ),
            SafeArea(
              child: AnimatedPadding(
                duration: const Duration(milliseconds: 180),
                padding: EdgeInsets.only(bottom: bottomInset > 0 ? 12 : 0),
                child: Center(
                  child: SingleChildScrollView(
                    padding: const EdgeInsets.fromLTRB(20, 24, 20, 24),
                    child: ConstrainedBox(
                      constraints: const BoxConstraints(maxWidth: 460),
                      child: Container(
                        padding: const EdgeInsets.fromLTRB(22, 24, 22, 20),
                        decoration: BoxDecoration(
                          color: Colors.white.withOpacity(0.94),
                          borderRadius: BorderRadius.circular(28),
                          border: Border.all(
                            color: Colors.white.withOpacity(0.55),
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
                        child: Form(
                          key: _formKey,
                          child: Column(
                            mainAxisSize: MainAxisSize.min,
                            crossAxisAlignment: CrossAxisAlignment.stretch,
                            children: [
                              const SizedBox(height: 6),
                              _buildHeader(),
                              const SizedBox(height: 22),
                              _inputField(
                                controller: _username,
                                hintText: 'Korisničko ime',
                                icon: Icons.person_rounded,
                                keyboardType: TextInputType.text,
                                autofillHints: const [AutofillHints.username],
                                validator: (v) {
                                  if (v == null || v.trim().isEmpty) {
                                    return 'Unesi korisničko ime';
                                  }
                                  if (v.trim().length < 3) {
                                    return 'Korisničko ime je prekratko';
                                  }
                                  return null;
                                },
                              ),
                              const SizedBox(height: 14),
                              _inputField(
                                controller: _password,
                                hintText: 'Lozinka',
                                icon: Icons.lock_rounded,
                                obscureText: _obscure,
                                keyboardType: TextInputType.visiblePassword,
                                autofillHints: const [AutofillHints.password],
                                suffix: IconButton(
                                  onPressed: () {
                                    setState(() => _obscure = !_obscure);
                                  },
                                  icon: Icon(
                                    _obscure
                                        ? Icons.visibility_off_rounded
                                        : Icons.visibility_rounded,
                                    color: LoginScreen.subText,
                                  ),
                                ),
                                validator: (v) {
                                  if (v == null || v.isEmpty) {
                                    return 'Unesi lozinku';
                                  }
                                  if (v.length < 4) {
                                    return 'Lozinka je prekratka';
                                  }
                                  return null;
                                },
                              ),
                              const SizedBox(height: 12),
                              Align(
                                alignment: Alignment.centerRight,
                                child: TextButton(
                                  onPressed: () {
                                    SnackbarHelper.showInfo(
                                      context,
                                      'Forgot password ekran još nije dodat.',
                                    );
                                  },
                                  child: const Text(
                                    'Zaboravljena lozinka?',
                                    style: TextStyle(
                                      fontWeight: FontWeight.w700,
                                      color: LoginScreen.darkPurple,
                                    ),
                                  ),
                                ),
                              ),
                              const SizedBox(height: 6),
                              SizedBox(
                                height: 56,
                                child: ElevatedButton(
                                  onPressed: _isLoading ? null : _login,
                                  style: ElevatedButton.styleFrom(
                                    elevation: 0,
                                    backgroundColor: LoginScreen.darkPurple,
                                    foregroundColor: Colors.white,
                                    shape: RoundedRectangleBorder(
                                      borderRadius: BorderRadius.circular(18),
                                    ),
                                  ),
                                  child: _isLoading
                                      ? const SizedBox(
                                          width: 22,
                                          height: 22,
                                          child: CircularProgressIndicator(
                                            strokeWidth: 2,
                                            color: Colors.white,
                                          ),
                                        )
                                      : const Text(
                                          'Uloguj se',
                                          style: TextStyle(
                                            fontSize: 18,
                                            fontWeight: FontWeight.w800,
                                          ),
                                        ),
                                ),
                              ),
                              const SizedBox(height: 18),
                              Row(
                                mainAxisAlignment: MainAxisAlignment.center,
                                children: [
                                  const Text(
                                    'Nemaš račun?',
                                    style: TextStyle(
                                      color: LoginScreen.subText,
                                      fontWeight: FontWeight.w600,
                                    ),
                                  ),
                                  TextButton(
                                    onPressed: () {
                                      Navigator.pushNamed(
                                        context,
                                        AppRoutes.register,
                                      );
                                    },
                                    child: const Text(
                                      'Registruj se',
                                      style: TextStyle(
                                        fontWeight: FontWeight.w900,
                                        color: LoginScreen.darkPurple,
                                      ),
                                    ),
                                  ),
                                ],
                              ),
                              const SizedBox(height: 6),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildHeader() {
    return Column(
      children: [
        Container(
          width: 82,
          height: 82,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            gradient: const LinearGradient(
              colors: [
                LoginScreen.primaryPurple,
                LoginScreen.darkPurple,
              ],
            ),
            boxShadow: [
              BoxShadow(
                color: LoginScreen.primaryPurple.withOpacity(0.30),
                blurRadius: 20,
                offset: const Offset(0, 10),
              ),
            ],
          ),
          child: const Icon(
            Icons.graphic_eq_rounded,
            color: Colors.white,
            size: 40,
          ),
        ),
        const SizedBox(height: 14),
        const Text(
          'GrooveOn',
          textAlign: TextAlign.center,
          style: TextStyle(
            fontSize: 32,
            fontWeight: FontWeight.w900,
            letterSpacing: 0.8,
            color: LoginScreen.darkPurple,
          ),
        ),
        const SizedBox(height: 6),
        const Text(
          'Feel the music. Live the vibe.',
          textAlign: TextAlign.center,
          style: TextStyle(
            fontSize: 14,
            fontWeight: FontWeight.w600,
            color: LoginScreen.subText,
          ),
        ),
      ],
    );
  }

  Widget _inputField({
    required TextEditingController controller,
    required String hintText,
    required IconData icon,
    TextInputType keyboardType = TextInputType.text,
    bool obscureText = false,
    Widget? suffix,
    List<String>? autofillHints,
    String? Function(String?)? validator,
  }) {
    return TextFormField(
      controller: controller,
      keyboardType: keyboardType,
      autofillHints: autofillHints,
      obscureText: obscureText,
      style: const TextStyle(
        fontSize: 16,
        fontWeight: FontWeight.w700,
        color: LoginScreen.textDark,
      ),
      validator: validator,
      decoration: InputDecoration(
        hintText: hintText,
        hintStyle: const TextStyle(
          color: Color(0xFF8A8A8A),
          fontWeight: FontWeight.w500,
        ),
        filled: true,
        fillColor: const Color(0xFFF8F6FB),
        prefixIcon: Icon(icon, color: LoginScreen.primaryPurple),
        suffixIcon: suffix,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 14,
          vertical: 18,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(18),
          borderSide: BorderSide.none,
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(18),
          borderSide: const BorderSide(
            color: Color(0xFFE7DDF0),
            width: 1.1,
          ),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(18),
          borderSide: const BorderSide(
            color: LoginScreen.darkPurple,
            width: 2,
          ),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(18),
          borderSide: const BorderSide(
            color: Colors.redAccent,
            width: 1.5,
          ),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(18),
          borderSide: const BorderSide(
            color: Colors.redAccent,
            width: 2,
          ),
        ),
      ),
    );
  }
}
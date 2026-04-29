import 'package:flutter/material.dart';

enum TriConfirmResult { cancel, bad, good }

class ConfirmDialogs {
  ConfirmDialogs._();

  static const Color _primary = Color(0xFF9C27B0);
  static const Color _primaryDark = Color(0xFF4A148C);
  static const Color _primarySoft = Color(0xFFEAD7F2);
  static const Color _premiumGold = Color(0xFFFFC857);
  static const Color _premiumGoldDark = Color(0xFFB8860B);
  static const Color _lockedPurple = Color(0xFF6A1B9A);
  static const Color _text = Color(0xFF1C1C1C);
  static const Color _muted = Color(0xFF6E6E6E);

  static const double _radius = 14;

  static Future<T?> _baseDialog<T>(
    BuildContext context, {
    required String title,
    required String message,
    required List<Widget> actions,
    bool barrierDismissible = false,
    Color? headerColor,
    Color? headerEndColor,
    IconData? headerIcon,
  }) {
    final Color startColor = headerColor ?? _primaryDark;
    final Color endColor = headerEndColor ?? _primary;

    return showDialog<T>(
      context: context,
      barrierDismissible: barrierDismissible,
      barrierColor: Colors.black.withOpacity(0.45),
      builder: (_) {
        return Dialog(
          backgroundColor: Colors.white,
          shape: RoundedRectangleBorder(
            borderRadius: BorderRadius.circular(_radius),
          ),
          child: SizedBox(
            width: 420,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.symmetric(
                    horizontal: 20,
                    vertical: 18,
                  ),
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.centerLeft,
                      end: Alignment.centerRight,
                      colors: [startColor, endColor],
                    ),
                    borderRadius: const BorderRadius.vertical(
                      top: Radius.circular(_radius),
                    ),
                  ),
                  child: Row(
                    children: [
                      if (headerIcon != null) ...[
                        Icon(headerIcon, color: Colors.white, size: 20),
                        const SizedBox(width: 10),
                      ],
                      Expanded(
                        child: Text(
                          title,
                          style: const TextStyle(
                            color: Colors.white,
                            fontSize: 18,
                            fontWeight: FontWeight.w800,
                          ),
                        ),
                      ),
                    ],
                  ),
                ),

                Padding(
                  padding: const EdgeInsets.fromLTRB(22, 22, 22, 18),
                  child: Text(
                    message,
                    style: const TextStyle(
                      fontSize: 14.5,
                      height: 1.45,
                      fontWeight: FontWeight.w600,
                      color: _text,
                    ),
                  ),
                ),

                Padding(
                  padding: const EdgeInsets.fromLTRB(16, 0, 16, 18),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.end,
                    children: actions,
                  ),
                ),
              ],
            ),
          ),
        );
      },
    );
  }

  static ButtonStyle _outlineBtn({required Color color}) {
    return OutlinedButton.styleFrom(
      foregroundColor: color,
      side: BorderSide(color: color, width: 1.2),
      padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 12),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10),
      ),
    );
  }

  static ButtonStyle _filledBtn({
    required Color bg,
    Color fg = Colors.white,
  }) {
    return ElevatedButton.styleFrom(
      backgroundColor: bg,
      foregroundColor: fg,
      elevation: 0,
      padding: const EdgeInsets.symmetric(horizontal: 18, vertical: 12),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(10),
      ),
    );
  }

  static Future<bool> yesNoConfirmation(
    BuildContext context, {
    required String question,
    String title = 'Potvrda',
    String yesText = 'Da',
    String noText = 'Ne',
    bool barrierDismissible = false,
    bool danger = false,
  }) async {
    final res = await _baseDialog<bool>(
      context,
      title: title,
      message: question,
      barrierDismissible: barrierDismissible,
      headerColor: danger ? _lockedPurple : _primaryDark,
      headerEndColor: danger ? _primary : _primary,
      headerIcon:
          danger ? Icons.lock_outline_rounded : Icons.help_outline_rounded,
      actions: [
        OutlinedButton(
          onPressed: () => Navigator.of(context).pop(false),
          style: _outlineBtn(color: _muted),
          child: Text(
            noText,
            style: const TextStyle(fontWeight: FontWeight.w800),
          ),
        ),
        const SizedBox(width: 12),
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(true),
          style: _filledBtn(bg: danger ? _lockedPurple : _primaryDark),
          child: Text(
            yesText,
            style: const TextStyle(fontWeight: FontWeight.w800),
          ),
        ),
      ],
    );

    return res ?? false;
  }

  static Future<void> okConfirmation(
    BuildContext context, {
    required String message,
    String title = 'Informacija',
    String okText = 'OK',
    bool barrierDismissible = false,
    bool danger = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: danger ? _lockedPurple : _primaryDark,
      headerEndColor: danger ? _primary : _primary,
      headerIcon:
          danger ? Icons.lock_outline_rounded : Icons.info_outline_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: danger ? _lockedPurple : _primaryDark),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w800),
          ),
        ),
      ],
    );
  }

  static Future<bool> badGoodConfirmation(
    BuildContext context, {
    required String question,
    String title = 'Potvrda',
    required String goodText,
    required String badText,
    bool barrierDismissible = false,
    bool goodIsPrimary = true,
  }) async {
    final res = await _baseDialog<bool>(
      context,
      title: title,
      message: question,
      barrierDismissible: barrierDismissible,
      headerColor: _primaryDark,
      headerEndColor: _primary,
      headerIcon: Icons.workspace_premium_rounded,
      actions: [
        OutlinedButton(
          onPressed: () => Navigator.of(context).pop(false),
          style: _outlineBtn(color: _muted),
          child: Text(
            badText,
            style: const TextStyle(fontWeight: FontWeight.w800),
          ),
        ),
        const SizedBox(width: 12),
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(true),
          style: _filledBtn(
            bg: goodIsPrimary ? _primaryDark : _primary,
          ),
          child: Text(
            goodText,
            style: const TextStyle(fontWeight: FontWeight.w800),
          ),
        ),
      ],
    );

    return res ?? false;
  }

  static Future<void> premiumLockedDialog(
    BuildContext context, {
    String title = "Premium content",
    String message =
        "This feature is available only to premium users.\n\nActivate premium and unlock all GrooveOn app features.",
    String okText = "U redu",
    bool barrierDismissible = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: _lockedPurple,
      headerEndColor: _primary,
      headerIcon: Icons.lock_outline_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: _lockedPurple),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w800),
          ),
        ),
      ],
    );
  }

  static Future<void> paymentSuccessDialog(
    BuildContext context, {
    String title = "Premium aktiviran",
    String message =
        "Your payment has been recorded successfully and premium access is active.",
    String okText = "Nastavi",
    bool barrierDismissible = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: _primaryDark,
      headerEndColor: _primary,
      headerIcon: Icons.check_circle_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: _primary),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
      ],
    );
  }

  static Future<void> paymentFailedDialog(
    BuildContext context, {
    String title = "Payment was not completed",
    String message =
        "The premium payment was not completed successfully. Try again or check your card details.",
    String okText = "U redu",
    bool barrierDismissible = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: _lockedPurple,
      headerEndColor: _primary,
      headerIcon: Icons.credit_card_off_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: _lockedPurple),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
      ],
    );
  }

  static Future<void> paymentCanceledDialog(
    BuildContext context, {
    String title = "Uplata otkazana",
    String message =
        "The payment process was canceled. Premium was not charged and no changes were made to your account.",
    String okText = "U redu",
    bool barrierDismissible = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: _lockedPurple,
      headerEndColor: _primary,
      headerIcon: Icons.remove_circle_outline_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: _lockedPurple),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
      ],
    );
  }

  static Future<void> paymentPendingDialog(
    BuildContext context, {
    String title = "Payment check",
    String message =
        "Payment has started, but the status has not been confirmed yet. Please wait a few seconds and check again.",
    String okText = "U redu",
    bool barrierDismissible = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: _premiumGoldDark,
      headerEndColor: _premiumGold,
      headerIcon: Icons.hourglass_top_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: _premiumGoldDark),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
      ],
    );
  }

  static Future<void> paymentErrorDialog(
    BuildContext context, {
    String title = "Payment verification error",
    String message =
        "An error occurred while checking the premium payment status. Try again in a few moments.",
    String okText = "U redu",
    bool barrierDismissible = false,
  }) async {
    await _baseDialog<void>(
      context,
      title: title,
      message: message,
      barrierDismissible: barrierDismissible,
      headerColor: _lockedPurple,
      headerEndColor: _primary,
      headerIcon: Icons.sync_problem_rounded,
      actions: [
        ElevatedButton(
          onPressed: () => Navigator.of(context).pop(),
          style: _filledBtn(bg: _lockedPurple),
          child: Text(
            okText,
            style: const TextStyle(fontWeight: FontWeight.w700),
          ),
        ),
      ],
    );
  }
}
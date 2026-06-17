import 'package:flutter/material.dart';
import 'package:flutter_stripe/flutter_stripe.dart';
import 'package:grooveon_mobile/exceptions/ApiException.dart';
import 'package:grooveon_mobile/providers/payment_provider.dart';
import 'package:provider/provider.dart';

class StripePaymentHelper {
  /// Returns paymentId on success, null if user explicitly cancelled the sheet.
  /// Throws [ApiException] for API errors or unexpected Stripe failures.
  static Future<int?> paySubscription(
    BuildContext context, {
    required int userId,
    required int subscriptionPlanId,
  }) async {
    FocusManager.instance.primaryFocus?.unfocus();

    final provider = context.read<PaymentProvider>();

    // API errors (e.g. backend rejects intent creation) propagate as-is
    final result = await provider.createNewIntent({
      "userId": userId,
      "subscriptionPlanId": subscriptionPlanId,
    });

    await Stripe.instance.initPaymentSheet(
      paymentSheetParameters: SetupPaymentSheetParameters(
        merchantDisplayName: "GrooveOn",
        style: ThemeMode.light,
        paymentIntentClientSecret: result.clientSecret,
      ),
    );

    try {
      await Stripe.instance.presentPaymentSheet();
    } on StripeException catch (e) {
      if (e.error.code == FailureCode.Canceled) {
        return null; // user dismissed the sheet — silent, caller decides message
      }
      throw ApiException(
        e.error.localizedMessage ?? e.error.message ?? "Payment processing failed. Please try again.",
      );
    }

    return result.paymentId;
  }
}

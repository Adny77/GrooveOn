import 'package:flutter/material.dart';
import 'package:flutter_stripe/flutter_stripe.dart';
import 'package:provider/provider.dart';

import 'package:grooveon_mobile/providers/payment_provider.dart';

class StripePaymentHelper {
  static Future<bool> paySubscription(
    BuildContext context, {
    required int userId,
    required int subscriptionPlanId,
  }) async {
    try {
      FocusManager.instance.primaryFocus?.unfocus();

      final provider = context.read<PaymentProvider>();

      final clientSecret = await provider.createNewIntent({
        "userId": userId,
        "subscriptionPlanId": subscriptionPlanId,
      });

      await Stripe.instance.initPaymentSheet(
        paymentSheetParameters: SetupPaymentSheetParameters(
          merchantDisplayName: "GrooveOn",
          style: ThemeMode.light,
          paymentIntentClientSecret: clientSecret,
        ),
      );

      await Stripe.instance.presentPaymentSheet();

      return true;
    } on StripeException catch (e) {
      debugPrint(
        "StripeException: ${e.error.localizedMessage ?? e.error.message}",
      );
      return false;
    } catch (e) {
      debugPrint("Stripe payment error: $e");
      return false;
    }
  }
}
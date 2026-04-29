import 'package:flutter/material.dart';
import 'package:grooveon_mobile/helper/snackBar_helper.dart';
import 'package:grooveon_mobile/helper/stripe_payment_helper.dart';
import 'package:grooveon_mobile/models/subscription_plan_response.dart';
import 'package:grooveon_mobile/models/subscription_response.dart';
import 'package:grooveon_mobile/providers/subscription_plan_provider.dart';
import 'package:grooveon_mobile/providers/subscription_provider.dart';
import 'package:grooveon_mobile/utils/Session.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

class SubscriptionScreen extends StatefulWidget {
  const SubscriptionScreen({super.key});

  @override
  State<SubscriptionScreen> createState() => _SubscriptionScreenState();
}

class _SubscriptionScreenState extends State<SubscriptionScreen> {
  static const Color primary = Color(0xFF9C27B0);

  bool _isLoading = true;
  bool _isPaying = false;

  SubscriptionResponse? _activeSubscription;
  List<SubscriptionPlanResponse> _plans = [];

  bool get _hasActiveSubscription => _activeSubscription != null;

  bool get _hasLockedSubscription {
    final sub = _activeSubscription;
    if (sub == null) return false;
    return sub.subscriptionPlanId != 1;
  }

  @override
  void initState() {
    super.initState();
    _loadData();
  }

  Future<void> _loadData() async {
    try {
      final subscriptionProvider = context.read<SubscriptionProvider>();
      final planProvider = context.read<SubscriptionPlanProvider>();

      final active = await subscriptionProvider.getMyActive();

      final result = await planProvider.get(filter: {
        "isActive": true,
        "retrieveAll": true,
      });

      if (!mounted) return;

      setState(() {
        _activeSubscription = active;
        _plans = result.items;
        _isLoading = false;
      });
    } catch (e) {
      if (!mounted) return;

      setState(() => _isLoading = false);

     SnackbarHelper.showError(context, e.toString());
    }
  }

  Future<void> _payForPlan(SubscriptionPlanResponse plan) async {
  if (_isPaying) return;

  if (_hasLockedSubscription) {
    SnackbarHelper.showError(
      context,
      "You already have an active subscription.",
    );
    return;
  }

  final userId = Session.userId;

  if (userId == null) {
    SnackbarHelper.showError(context, "User is not logged in.");
    return;
  }

  final confirm = await showDialog<bool>(
    context: context,
    builder: (_) => AlertDialog(
      title: const Text("Payment confirmation"),
      content: Text(
        "Do you want to purchase the '${plan.name}' plan for €${plan.price.toStringAsFixed(2)}?",
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context, false),
          child: const Text("Cancel"),
        ),
        ElevatedButton(
          onPressed: () => Navigator.pop(context, true),
          style: ElevatedButton.styleFrom(
            backgroundColor: primary,
            foregroundColor: Colors.white,
          ),
          child: const Text("Continue"),
        ),
      ],
    ),
  );

  if (confirm != true) return;

  setState(() => _isPaying = true);

  final success = await StripePaymentHelper.paySubscription(
    context,
    userId: userId,
    subscriptionPlanId: plan.id,
  );

  if (!mounted) return;

  setState(() => _isPaying = false);

  if (success) {
    SnackbarHelper.showSuccess(context, "Payment started successfully.");

    await _loadData();
  } else {
    SnackbarHelper.showError(context, "Payment was not completed.");
  }
}

  @override
  Widget build(BuildContext context) {
    if (_isLoading) {
      return const Scaffold(
        backgroundColor: Color(0xFFF8F5FA),
        body: Center(
          child: CircularProgressIndicator(color: primary),
        ),
      );
    }

    return Scaffold(
      backgroundColor: const Color(0xFFF8F5FA),
      body: SafeArea(
        child: RefreshIndicator(
          color: primary,
          onRefresh: _loadData,
          child: ListView(
            padding: const EdgeInsets.fromLTRB(18, 20, 18, 110),
            children: [
              const Text(
                "Subscription",
                style: TextStyle(
                  fontSize: 28,
                  fontWeight: FontWeight.w800,
                  color: Colors.black87,
                ),
              ),
              const SizedBox(height: 6),
              const Text(
                "Upgrade your GrooveOn experience.",
                style: TextStyle(
                  fontSize: 15,
                  color: Colors.black54,
                ),
              ),
              const SizedBox(height: 24),
              _currentPlanCard(),
              const SizedBox(height: 24),
              ..._plans.map((plan) {
                final isCurrent =
                    _activeSubscription?.subscriptionPlanId == plan.id;

                return Padding(
                  padding: const EdgeInsets.only(bottom: 16),
                  child: _planCard(
                    plan: plan,
                    isCurrent: isCurrent,
                    onTap: () => _payForPlan(plan),
                  ),
                );
              }),
            ],
          ),
        ),
      ),
    );
  }

  Widget _currentPlanCard() {
    final sub = _activeSubscription;

    final planName = sub?.subscriptionPlanName ?? "No active plan";
    final validText = _subscriptionValidityText(sub);

    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        gradient: const LinearGradient(
          colors: [
            Color(0xFF9C27B0),
            Color(0xFF6A1B9A),
          ],
        ),
        borderRadius: BorderRadius.circular(22),
        boxShadow: const [
          BoxShadow(
            color: Color(0x229C27B0),
            blurRadius: 14,
            offset: Offset(0, 8),
          )
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Icon(
            Icons.workspace_premium_rounded,
            color: Colors.white,
            size: 34,
          ),
          const SizedBox(height: 16),
          const Text(
            "Your current plan",
            style: TextStyle(
              color: Colors.white70,
              fontSize: 14,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            planName,
            style: const TextStyle(
              color: Colors.white,
              fontSize: 24,
              fontWeight: FontWeight.w800,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            validText,
            style: const TextStyle(
              color: Colors.white70,
              fontSize: 14,
              height: 1.4,
            ),
          ),
        ],
      ),
    );
  }

  Widget _planCard({
    required SubscriptionPlanResponse plan,
    required bool isCurrent,
    required VoidCallback onTap,
  }) {
    final isPremium = plan.price > 0;
    final isRecommended = plan.id == 2;

    final isBlockedByActiveSubscription =
        _hasLockedSubscription && !isCurrent;

    final currentInfo =
        isCurrent ? _subscriptionValidityText(_activeSubscription) : null;

    return Container(
      padding: const EdgeInsets.all(18),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(22),
        border: Border.all(
          color: isCurrent
              ? primary
              : isRecommended
                  ? const Color(0x999C27B0)
                  : const Color(0xFFE9E1EE),
          width: isCurrent ? 2.4 : 1.2,
        ),
        boxShadow: [
          BoxShadow(
            color: isCurrent
                ? const Color(0x229C27B0)
                : const Color(0x0F000000),
            blurRadius: isCurrent ? 18 : 12,
            offset: const Offset(0, 6),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              if (isCurrent)
                _tag(
                  text: "Current plan",
                  icon: Icons.check_circle_rounded,
                  filled: true,
                ),
              if (!isCurrent && isRecommended)
                _tag(
                  text: "Recommended",
                  icon: Icons.star_rounded,
                  filled: false,
                ),
            ],
          ),
          if (isCurrent || isRecommended) const SizedBox(height: 12),
          Text(
            plan.name,
            style: const TextStyle(
              fontSize: 21,
              fontWeight: FontWeight.w800,
              color: Colors.black87,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            isPremium ? "€${plan.price.toStringAsFixed(2)} / month" : "Free",
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w700,
              color: isCurrent || isRecommended ? primary : Colors.black87,
            ),
          ),
          const SizedBox(height: 8),
          Text(
            plan.description ?? "",
            style: const TextStyle(
              color: Colors.black54,
              height: 1.4,
            ),
          ),
          if (currentInfo != null) ...[
            const SizedBox(height: 14),
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(12),
              decoration: BoxDecoration(
                color: const Color(0xFFF5E8F8),
                borderRadius: BorderRadius.circular(14),
              ),
              child: Text(
                currentInfo,
                style: const TextStyle(
                  color: primary,
                  fontWeight: FontWeight.w700,
                  height: 1.35,
                ),
              ),
            ),
          ],
          const SizedBox(height: 16),
          ..._featuresForPlan(plan).map(
            (feature) => Padding(
              padding: const EdgeInsets.only(bottom: 9),
              child: Row(
                children: [
                  const Icon(
                    Icons.check_circle_rounded,
                    color: primary,
                    size: 19,
                  ),
                  const SizedBox(width: 9),
                  Expanded(
                    child: Text(
                      feature,
                      style: const TextStyle(
                        color: Colors.black87,
                        fontSize: 14,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            width: double.infinity,
            height: 48,
            child: ElevatedButton(
              onPressed: isCurrent ||
                      isBlockedByActiveSubscription ||
                      _isPaying ||
                      !isPremium
                  ? null
                  : onTap,
              style: ElevatedButton.styleFrom(
                backgroundColor: isRecommended ? primary : Colors.black87,
                disabledBackgroundColor: const Color(0xFFE9E1EE),
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                elevation: 0,
              ),
              child: Text(
                isCurrent
                    ? "Active"
                    : isBlockedByActiveSubscription
                        ? "Locked"
                        : !isPremium
                            ? "Free plan"
                            : _isPaying
                                ? "Processing..."
                                : "Upgrade",
                style: TextStyle(
                  color: isCurrent ||
                          isBlockedByActiveSubscription ||
                          !isPremium
                      ? Colors.black45
                      : Colors.white,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _tag({
    required String text,
    required IconData icon,
    required bool filled,
  }) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(
        color: filled ? primary : const Color(0x159C27B0),
        borderRadius: BorderRadius.circular(20),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(
            icon,
            size: 15,
            color: filled ? Colors.white : primary,
          ),
          const SizedBox(width: 5),
          Text(
            text,
            style: TextStyle(
              color: filled ? Colors.white : primary,
              fontWeight: FontWeight.w700,
              fontSize: 12,
            ),
          ),
        ],
      ),
    );
  }

  String _subscriptionValidityText(SubscriptionResponse? sub) {
    if (sub == null) {
      return "No active subscription found.";
    }

    final start = _formatDate(sub.startDate);

    if (sub.expiryDate == null) {
      return "Active from $start. This plan does not expire.";
    }

    final end = _formatDate(sub.expiryDate!);

    return "Active from $start to $end.";
  }

  String _formatDate(DateTime date) {
    return DateFormat("dd.MM.yyyy").format(date);
  }

  List<String> _featuresForPlan(SubscriptionPlanResponse plan) {
    if (plan.id == 1) {
      return const [
        "Limited playlists",
        "No access to album or artist",
        "Random feature",
      ];
    }

    if (plan.id == 2) {
      return const [
        "Premium listening access",
        "Unlimited playlist usage",
        "Access to artists and albums",
      ];
    }

    return const [
      "Everything from Premium",
      "Priority features",
      "Advanced account benefits",
    ];
  }
}
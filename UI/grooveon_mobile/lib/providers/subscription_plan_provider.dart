import 'package:grooveon_mobile/models/subscription_plan_response.dart';
import 'package:grooveon_mobile/providers/base_provider.dart';

class SubscriptionPlanProvider extends BaseProvider<SubscriptionPlanResponse> {
  SubscriptionPlanProvider() : super("SubscriptionPlan");

  @override
  SubscriptionPlanResponse fromJson(dynamic json) {
    return SubscriptionPlanResponse.fromJson(json);
  }
}
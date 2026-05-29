import 'package:grooveon_desktop/models/response/subscription_plan_response.dart';
import 'base_provider.dart';

class SubscriptionPlanProvider extends BaseProvider<SubscriptionPlanResponse> {
  SubscriptionPlanProvider() : super("SubscriptionPlan");

  @override
  SubscriptionPlanResponse fromJson(data) {
    return SubscriptionPlanResponse.fromJson(data);
  }
}

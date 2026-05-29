import 'package:json_annotation/json_annotation.dart';

part 'subscription_plan_response.g.dart';

@JsonSerializable()
class SubscriptionPlanResponse {
  final int id;
  final String name;
  final String? planCode;
  final double price;
  final int durationDays;
  final String? description;
  final bool isActive;

  SubscriptionPlanResponse({
    required this.id,
    required this.name,
    this.planCode,
    required this.price,
    required this.durationDays,
    this.description,
    required this.isActive,
  });

  factory SubscriptionPlanResponse.fromJson(Map<String, dynamic> json) =>
      _$SubscriptionPlanResponseFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionPlanResponseToJson(this);
}

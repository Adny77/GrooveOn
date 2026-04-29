import 'package:json_annotation/json_annotation.dart';

part 'subscription_response.g.dart';

@JsonSerializable()
class SubscriptionResponse {
  final int id;
  final int userId;
  final String? username;
  final String? userFullName;
  final int subscriptionPlanId;
  final String? subscriptionPlanName;
  final double subscriptionPlanPrice;
  final int subscriptionPlanDurationDays;
  final DateTime startDate;
  final DateTime? expiryDate;
  final bool isActive;
  final bool isExpired;

  SubscriptionResponse({
    required this.id,
    required this.userId,
    this.username,
    this.userFullName,
    required this.subscriptionPlanId,
    this.subscriptionPlanName,
    required this.subscriptionPlanPrice,
    required this.subscriptionPlanDurationDays,
    required this.startDate,
    this.expiryDate,
    required this.isActive,
    required this.isExpired,
  });

  factory SubscriptionResponse.fromJson(Map<String, dynamic> json) =>
      _$SubscriptionResponseFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionResponseToJson(this);
}
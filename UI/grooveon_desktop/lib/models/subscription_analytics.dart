import 'package:json_annotation/json_annotation.dart';

part 'subscription_analytics.g.dart';

@JsonSerializable()
class SubscriptionAnalytics {
  final int basicCount;
  final int premiumCount;
  final double basicPercentage;
  final double premiumPercentage;
  final int totalCount;
  final String periodLabel;

  SubscriptionAnalytics({
    required this.basicCount,
    required this.premiumCount,
    required this.basicPercentage,
    required this.premiumPercentage,
    required this.totalCount,
    required this.periodLabel,
  });

  factory SubscriptionAnalytics.fromJson(Map<String, dynamic> json) =>
      _$SubscriptionAnalyticsFromJson(json);

  Map<String, dynamic> toJson() => _$SubscriptionAnalyticsToJson(this);
}
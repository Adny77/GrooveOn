// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_analytics.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionAnalytics _$SubscriptionAnalyticsFromJson(
  Map<String, dynamic> json,
) => SubscriptionAnalytics(
  basicCount: (json['basicCount'] as num).toInt(),
  premiumCount: (json['premiumCount'] as num).toInt(),
  basicPercentage: (json['basicPercentage'] as num).toDouble(),
  premiumPercentage: (json['premiumPercentage'] as num).toDouble(),
  totalCount: (json['totalCount'] as num).toInt(),
  periodLabel: json['periodLabel'] as String,
);

Map<String, dynamic> _$SubscriptionAnalyticsToJson(
  SubscriptionAnalytics instance,
) => <String, dynamic>{
  'basicCount': instance.basicCount,
  'premiumCount': instance.premiumCount,
  'basicPercentage': instance.basicPercentage,
  'premiumPercentage': instance.premiumPercentage,
  'totalCount': instance.totalCount,
  'periodLabel': instance.periodLabel,
};

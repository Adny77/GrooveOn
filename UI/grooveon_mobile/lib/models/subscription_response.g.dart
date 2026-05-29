// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionResponse _$SubscriptionResponseFromJson(
  Map<String, dynamic> json,
) => SubscriptionResponse(
  id: (json['id'] as num).toInt(),
  userId: (json['userId'] as num).toInt(),
  username: json['username'] as String?,
  userFullName: json['userFullName'] as String?,
  subscriptionPlanId: (json['subscriptionPlanId'] as num).toInt(),
  subscriptionPlanName: json['subscriptionPlanName'] as String?,
  subscriptionPlanCode: json['subscriptionPlanCode'] as String?,
  subscriptionPlanPrice: (json['subscriptionPlanPrice'] as num).toDouble(),
  subscriptionPlanDurationDays: (json['subscriptionPlanDurationDays'] as num)
      .toInt(),
  startDate: DateTime.parse(json['startDate'] as String),
  expiryDate: json['expiryDate'] == null
      ? null
      : DateTime.parse(json['expiryDate'] as String),
  isActive: json['isActive'] as bool,
  isExpired: json['isExpired'] as bool,
);

Map<String, dynamic> _$SubscriptionResponseToJson(
  SubscriptionResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'userId': instance.userId,
  'username': instance.username,
  'userFullName': instance.userFullName,
  'subscriptionPlanId': instance.subscriptionPlanId,
  'subscriptionPlanName': instance.subscriptionPlanName,
  'subscriptionPlanCode': instance.subscriptionPlanCode,
  'subscriptionPlanPrice': instance.subscriptionPlanPrice,
  'subscriptionPlanDurationDays': instance.subscriptionPlanDurationDays,
  'startDate': instance.startDate.toIso8601String(),
  'expiryDate': instance.expiryDate?.toIso8601String(),
  'isActive': instance.isActive,
  'isExpired': instance.isExpired,
};

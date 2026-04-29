// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'subscription_plan_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SubscriptionPlanResponse _$SubscriptionPlanResponseFromJson(
  Map<String, dynamic> json,
) => SubscriptionPlanResponse(
  id: (json['id'] as num).toInt(),
  name: json['name'] as String,
  price: (json['price'] as num).toDouble(),
  durationDays: (json['durationDays'] as num).toInt(),
  description: json['description'] as String?,
  isActive: json['isActive'] as bool,
);

Map<String, dynamic> _$SubscriptionPlanResponseToJson(
  SubscriptionPlanResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'price': instance.price,
  'durationDays': instance.durationDays,
  'description': instance.description,
  'isActive': instance.isActive,
};

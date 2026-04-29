// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'payment_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PaymentResponse _$PaymentResponseFromJson(Map<String, dynamic> json) =>
    PaymentResponse(
      id: (json['id'] as num).toInt(),
      subscriptionId: (json['subscriptionId'] as num).toInt(),
      paymentStatus: json['paymentStatus'] as String,
      stripePaymentIntentId: json['stripePaymentIntentId'] as String?,
      createdAt: DateTime.parse(json['createdAt'] as String),
      paidAt: json['paidAt'] == null
          ? null
          : DateTime.parse(json['paidAt'] as String),
      failureReason: json['failureReason'] as String?,
      paymentMethod: json['paymentMethod'] as String?,
      paymentAmount: (json['paymentAmount'] as num).toDouble(),
      paymentDate: json['paymentDate'] == null
          ? null
          : DateTime.parse(json['paymentDate'] as String),
      userId: (json['userId'] as num?)?.toInt(),
      username: json['username'] as String?,
      subscriptionPlanId: (json['subscriptionPlanId'] as num?)?.toInt(),
      subscriptionPlanName: json['subscriptionPlanName'] as String?,
    );

Map<String, dynamic> _$PaymentResponseToJson(PaymentResponse instance) =>
    <String, dynamic>{
      'id': instance.id,
      'subscriptionId': instance.subscriptionId,
      'paymentStatus': instance.paymentStatus,
      'stripePaymentIntentId': instance.stripePaymentIntentId,
      'createdAt': instance.createdAt.toIso8601String(),
      'paidAt': instance.paidAt?.toIso8601String(),
      'failureReason': instance.failureReason,
      'paymentMethod': instance.paymentMethod,
      'paymentAmount': instance.paymentAmount,
      'paymentDate': instance.paymentDate?.toIso8601String(),
      'userId': instance.userId,
      'username': instance.username,
      'subscriptionPlanId': instance.subscriptionPlanId,
      'subscriptionPlanName': instance.subscriptionPlanName,
    };

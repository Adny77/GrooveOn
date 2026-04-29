import 'package:json_annotation/json_annotation.dart';

part 'payment_response.g.dart';

@JsonSerializable()
class PaymentResponse {
  final int id;
  final int subscriptionId;
  final String paymentStatus;
  final String? stripePaymentIntentId;
  final DateTime createdAt;
  final DateTime? paidAt;
  final String? failureReason;
  final String? paymentMethod;
  final double paymentAmount;
  final DateTime? paymentDate;

  final int? userId;
  final String? username;
  final int? subscriptionPlanId;
  final String? subscriptionPlanName;

  PaymentResponse({
    required this.id,
    required this.subscriptionId,
    required this.paymentStatus,
    this.stripePaymentIntentId,
    required this.createdAt,
    this.paidAt,
    this.failureReason,
    this.paymentMethod,
    required this.paymentAmount,
    this.paymentDate,
    this.userId,
    this.username,
    this.subscriptionPlanId,
    this.subscriptionPlanName,
  });

  factory PaymentResponse.fromJson(Map<String, dynamic> json) =>
      _$PaymentResponseFromJson(json);

  Map<String, dynamic> toJson() => _$PaymentResponseToJson(this);
}
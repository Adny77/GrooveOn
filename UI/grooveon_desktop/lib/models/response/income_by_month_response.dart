import 'package:json_annotation/json_annotation.dart';

part 'income_by_month_response.g.dart';

@JsonSerializable()
class IncomeByMonthResponse {
  final int month;
  final double totalIncome;

  IncomeByMonthResponse({
    required this.month,
    required this.totalIncome,
  });

  factory IncomeByMonthResponse.fromJson(Map<String, dynamic> json) =>
      _$IncomeByMonthResponseFromJson(json);

  Map<String, dynamic> toJson() => _$IncomeByMonthResponseToJson(this);
}
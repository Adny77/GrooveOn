import 'package:json_annotation/json_annotation.dart';

part 'answer_upsert_request.g.dart';

@JsonSerializable()
class AnswerUpsertRequest {
  final int questionId;
  final int adminId;
  final String message;

  AnswerUpsertRequest({
    required this.questionId,
    required this.adminId,
    required this.message,
  });

  factory AnswerUpsertRequest.fromJson(Map<String, dynamic> json) =>
      _$AnswerUpsertRequestFromJson(json);

  Map<String, dynamic> toJson() => _$AnswerUpsertRequestToJson(this);
}